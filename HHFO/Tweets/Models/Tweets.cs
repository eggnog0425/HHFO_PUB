using CoreTweet;
using CoreTweet.Core;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HHFO.Models.Data;
using System.Globalization;

namespace HHFO.Models
{
    public class Tweets: IDisposable
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();
        private List<Tweet> _tweets { get; set; } = new List<Tweet>();
        private ObservableCollection<Tweet> _showTweets { get; set; } = new ObservableCollection<Tweet>();
        public ReadOnlyReactiveCollection<Tweet> ShowTweets { get; private set; }
        public ReactivePropertySlim<bool> IsOrSearch { get; set; } = new ReactivePropertySlim<bool>(true);
        private ObservableCollection<Func<Tweet, bool>> Predicates { get; set; } = new ObservableCollection<Func<Tweet, bool>>();
        private ObservableCollection<Media> _medias { get; set; } = new ObservableCollection<Media>();
        public ReadOnlyReactiveCollection<Media> Medias { get; private set; }

        private bool Descend = false;
        private string SortKey = null;

        public Tweets()
        {
            ShowTweets = _showTweets.ToReadOnlyReactiveCollection();
            Medias = _medias.ToReadOnlyReactiveCollection();
            IsOrSearch.Subscribe(async _ => await Reflesh()).AddTo(Disposable);
            Predicates.CollectionChangedAsObservable().Subscribe(async _ => await Reflesh()).AddTo(Disposable);
        }

        private async Task Reflesh()
        {
            await Task.Run(() =>
            {
                RefleshShowTweets();
                ClearMedias();
                AddMedias();
            }).ConfigureAwait(false);
        }
        private void ClearMedias()
        {
            lock(_medias)
            {
                _medias.Clear();
            }
        }
        internal long MinId()
        {
            return _tweets.Min(t => t.Id);
        }

        private void RefleshShowTweets()
        {
            var filteredTweets = _tweets.Where(tweet => Filter(tweet));
            lock (_showTweets)
            {
                _showTweets.Clear();
                foreach (var tweet in Sort(filteredTweets))
                {
                    _showTweets.Add(tweet);
                }
            }
        }

        private void AddMedias()
        {
            var mediaTweets = _showTweets.Where(t => t.Media != null);
            var medias = mediaTweets.SelectMany(t => t.Media);
            lock (_medias)
            {
                var filteredMedias = medias.Where(newMedia => !_medias.Any(media => newMedia.MediaUrl == media.MediaUrl));
                // メディアは下手にソートして上に追加しようとすると見ている場所が下に流れてうざいのでソートしない
                foreach (var media in filteredMedias)
                {
                    _medias.Add(media);
                }
            }
        }

        public async Task<RateLimit> AddTweetsAsync(Task<ListedResponse<Status>> req)
        {
            var statuses = await req;
            var tweets = statuses.Select(s => new Tweet(s));
            lock (_tweets)
            {
                var filteredTweets = tweets.Where(newT => !_tweets.Any(t => t.Id == newT.Id));
                if (SortKey == null)
                {
                    _tweets.AddRange(filteredTweets);
                } else
                {
                    AddTweetWhenSorted(filteredTweets);
                }
            }
                RefleshShowTweets();
                AddMedias();
            return statuses.RateLimit;
        }

        private void AddTweetWhenSorted(IEnumerable<Tweet> addTweets)
        {
            // 追加の際は末尾から回す必要があるので追加要素をあらかじめソート
            var sortedAddTweets = Sort(addTweets).ToList();

            // 追加される側をぐるぐる回しつつ追加するやつと比較して該当箇所にぶっこみたい
            for(var i = _tweets.Count - 1; -1 < i; i--)
            {

                var compare = KeySelector(_tweets[i]).CompareTo(KeySelector(sortedAddTweets.Last()));
                // Descendの場合追加要素の方が小さかったらリストの対象箇所に挿入する
                if (Descend)
                {
                    if (compare < 0)
                    {
                        for(var j = sortedAddTweets.Count - 1; -1 < j ; j--)
                        {
                            _tweets.Insert(i + j, sortedAddTweets.Last());
                            sortedAddTweets.RemoveAt(j);
                        }
                    }
                }
                // ascendの場合追加要素の方が大きかったらリストの対象箇所に挿入する
            }
        }

        private void 

        Func<Tweet, Tweet, bool> AddRulePredicate()
        {
            switch (SortKey) {

                default:
                    throw new NotImplementedException();
            }
        }

        protected bool Filter(Tweet tweet)
        {
            if (Predicates.Count() == 0)
            {
                return true;
            }

            if (IsOrSearch.Value)
            {
                return Predicates.Any(predicate => predicate(tweet));
            }
            else
            {
                return Predicates.All(predicate => predicate(tweet));
            }
        }

        public void Dispose()
        {
            Disposable.Dispose();
        }

        internal async Task AddPredicates(Func<Tweet, bool> func)
        {
            await Task.Run(() =>Predicates.Add(func)).ConfigureAwait(false);
        }

        internal async Task RemovePredicates(Func<Tweet, bool> func)
        {
            await Task.Run(() =>Predicates.Remove(func)).ConfigureAwait(false);
        }
        private IOrderedEnumerable<Tweet> Sort(IEnumerable<Tweet> tweets)
        {
            return Descend
                 ? tweets.OrderByDescending(KeySelector)
                 : tweets.OrderBy(KeySelector);
        }

        private IComparable KeySelector(Tweet tweet)
        {
            var isRt = tweet.IsRetweetedTweet;
            return SortKey switch
            {
                nameof(tweet.Id) => tweet.Id,
                nameof(tweet.CreatedAt) => tweet.CreatedAt,
                nameof(tweet.UserId) => isRt ? tweet.RetweetedUserId : tweet.UserId,
                nameof(tweet.ScreenName) => isRt ? tweet.RetweetedUserScreenName : tweet.ScreenName,
                nameof(tweet.UserName) => isRt ? tweet.RetweetedUserName : tweet.UserName,
                nameof(tweet.FullText) => tweet.FullText,
                nameof(tweet.Source) => tweet.Source,
                _ => tweet.Id
            };
        }
    }
}
