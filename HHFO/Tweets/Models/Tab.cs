using CoreTweet;
using HHFO.Models.Logic.EventAggregator.Tweet;
using ImTools;
using NLog.Filters;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HHFO.Models
{
    public abstract class Tab : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; }

        public long Id { get; protected set; }
        public string Name { get; protected set; }

        // チェックボックス・ラジオボタンの状態
        public ReactiveProperty<bool> IsFilteredLink { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsFilteredImages { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsFilteredVideos { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsFilteredRetweeted { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsOrSearch { get; private set; } = new ReactiveProperty<bool>(true);

        protected Tokens Token { get; set; }

        /// <summary>
        /// APIから取得した全ツイート
        /// </summary>
        private List<Tweet> tweets { get; set; } = new List<Tweet>();
        protected List<Tweet> Tweets 
        { 
            get 
            { 
                return tweets; 
            } 
            set
            {
                tweets = value;
                RaisePropertyChanged(nameof(Tweets));
            }
        }

        /// <summary>
        /// 画面に実際に表示されるTweets
        /// </summary>
        public ObservableCollection<Tweet> ShowTweets { get; private set; } = new ObservableCollection<Tweet>();
        public ObservableCollection<Tweet> SelectedTweetItems { get; set; } = new ObservableCollection<Tweet>();

        protected ObservableCollection<Media> medias { get; private set; } = new ObservableCollection<Media>();
        /// <summary>
        /// mediasからShowTweetに含まれるIdの発言のみを抽出
        /// </summary>
        public ReactiveCollection<Media> Medias { get; private set; } = new ReactiveCollection<Media>();

        public ObservableCollection<Func<Tweet, bool>> Predicates { get; protected set; } = new ObservableCollection<Func<Tweet, bool>>();

        // チェックボックス・ラジオボタンエリアのコマンド
        public ReactiveCommand OnCheckFilterLink { get; }
        public ReactiveCommand OnCheckFilterImages { get; }
        public ReactiveCommand OnCheckFilterVideos { get; }
        public ReactiveCommand OnCheckFilterRetweeted { get; }
        public ReactiveCommand OnClickOrSearch { get; }
        public ReactiveCommand OnClickAndSearch { get; }
        public ReactiveCommand IsCheckedAndSearch { get; }
        public ReactiveCommand SendReply { get; }
        public ReactiveCommand<SelectionChangedEventArgs> SelectionChange { get; }

        public ReactiveCommand<KeyEventArgs> SaveImages { get; }
        public ReactiveCommand<SelectionChangedEventArgs> SelectionChangeMedia { get; }
        protected ITweetPublisher TweetPublisher { get; set; }

        /// <summary>
        /// 自動更新用のタイマー
        /// </summary>
        private DispatcherTimer timer { get; set; } = new DispatcherTimer(DispatcherPriority.Normal);

        // 表示形式の切替
        public ReactiveProperty<Visibility> NormalGridVisibility { get; } = new ReactiveProperty<Visibility>(Visibility.Visible);
        public ReactiveProperty<Visibility> MediaGridVisibility { get; } = new ReactiveProperty<Visibility>(Visibility.Collapsed);

        private Func<Tweet, bool> FilterLink = tweet => (tweet.Status.Entities?.Urls?.Length ?? 0) != 0;
        private Func<Tweet, bool> FilterImages = tweet => tweet.Status.ExtendedEntities?.Media?[0].Type == "photo" || tweet.Status.ExtendedEntities?.Media?[0].Type == "animated_gif";
        private Func<Tweet, bool> FilterVideos = tweet => tweet.Status.ExtendedEntities?.Media?[0].Type == "video";
        private Func<Tweet, bool> FilterRetweeted = tweet => tweet.Status.RetweetedStatus != null;

        public Tab(ITweetPublisher tweetPublisher)
        {
            Disposable = new CompositeDisposable();
            this.TweetPublisher = tweetPublisher;
            Token = Authorization.GetToken();

            timer.Interval = TimeSpan.FromSeconds(10.0);
            timer.Tick += (s, e) => FetchTweets();
            timer.Start();

            AddMedias(medias);

            this.PropertyChangedAsObservable()
                .Where(e => e.PropertyName == nameof(Tweets))
                .Subscribe(_ => OnTweetsChanged())
                .AddTo(Disposable);
            OnCheckFilterLink = new ReactiveCommand()
                .AddTo(Disposable);
            OnCheckFilterImages = new ReactiveCommand()
                .AddTo(Disposable);
            OnCheckFilterVideos = new ReactiveCommand()
                .AddTo(Disposable);
            OnCheckFilterRetweeted = new ReactiveCommand()
                .AddTo(Disposable);
            OnClickOrSearch = new ReactiveCommand()
                .AddTo(Disposable);
            OnClickAndSearch = new ReactiveCommand()
                .AddTo(Disposable);
            IsCheckedAndSearch = new ReactiveCommand()
                .AddTo(Disposable);
            SendReply = new ReactiveCommand()
                .AddTo(Disposable);
            SelectionChange = new ReactiveCommand<SelectionChangedEventArgs>()
                .AddTo(Disposable);
            SaveImages = new ReactiveCommand<KeyEventArgs>()
                .AddTo(Disposable);
            SelectionChangeMedia = new ReactiveCommand<SelectionChangedEventArgs>()
                .AddTo(Disposable);

            OnCheckFilterLink.Subscribe(_ => OnCheckFilterLinkAction())
                .AddTo(Disposable);
            OnCheckFilterImages.Subscribe(_ => OnCheckFilterImagesAction())
                .AddTo(Disposable);
            OnCheckFilterVideos.Subscribe(_ => OnCheckFilterVideosAction())
                .AddTo(Disposable);
            OnCheckFilterRetweeted.Subscribe(_ => OnCheckFilterRetweetedAction())
                .AddTo(Disposable);
            OnClickOrSearch.Subscribe(_ => OnClickOrSearchAction())
                .AddTo(Disposable);
            OnClickAndSearch.Subscribe(_ => OnClickAndSearchAction())
                .AddTo(Disposable);
            IsCheckedAndSearch.Subscribe(_ => IsCheckedAndSearchAction())
                .AddTo(Disposable);
            SendReply.Subscribe(_ => SendReplyAction())
                .AddTo(Disposable);
            SelectionChange.Subscribe(e => SelectionChangeAction(e))
                .AddTo(Disposable);
            SaveImages.Subscribe(e => SaveImagesAction(e))
                .AddTo(Disposable);
            SelectionChangeMedia.Subscribe(e => SelectionChangeMediaAction(e))
                .AddTo(Disposable);
        }

        private void SelectionChangeMediaAction(SelectionChangedEventArgs e)
        {
            foreach(var media in e.RemovedItems.Cast<Media>())
            {
                media.IsSelected.Value = false;
            }
            foreach(var media in e.AddedItems.Cast<Media>())
            {
                media.IsSelected.Value = true;
            }
        }

        private void SaveImagesAction(KeyEventArgs e)
        {
            var media = (e.Source as ListBoxItem).Content as Media;
            if (media == null || !(media.IsSelected.Value)) {
                return;
            }

            var client = new System.Net.WebClient();
            var regexPre = new Regex(@".*\.com/media/");
            var regexSuf = new Regex(@"\.");

            var fileName = regexPre.Replace(media.MediaUrl, "");
            fileName = "./images/" + regexSuf.Replace(fileName, "_orig.");
            if (File.Exists(fileName))
            {
                return;
            }
            client.DownloadFileAsync(new Uri(media.MediaUrl + ":orig"), fileName);

            /*
            var img = new Bitmap(fileName);
            var status = Tweets.FirstOrDefault(t => t.Status.Id == media.Id).Status;
            var author = new  List<>
            */
            
        }

        private void SelectionChangeAction(SelectionChangedEventArgs e)
        {
            e.OriginalSource.GetType().ToString();
        }

        private void SendReplyAction()
        {
            TweetPublisher.TweetId = SelectedTweetItems.Count == 1
                                   ? SelectedTweetItems[0].Status.Id
                                   : 0;
            TweetPublisher.UserScreenNames = SelectedTweetItems.Select(t => t.Status.User.ScreenName).ToList();
            TweetPublisher.Publish();
        }

        private void OnTweetsChanged()
        {
            Reflesh(Tweets);
            AddMedia();
        }

        private void AddMedia()
        {
            var newMedias = Tweets.Where(t => FilterImages(t) || FilterVideos(t))
                      .Select(t => new Media(id: t.Status.Id
                                           , type: t.Status.ExtendedEntities?.Media?[0].Type
                                           , mediaUrl: t.Status.ExtendedEntities?.Media?[0].MediaUrlHttps));
            foreach (var media in newMedias)
            {
                if (Medias.Any(m => m.Id == media.Id))
                {
                    continue;
                }
                Medias.Add(media);
            }
        }

        protected abstract void FetchTweets();

        protected void Reflesh(IEnumerable<Tweet> tweets)
        {
            ShowTweets.Clear();
            foreach (var tweet in tweets)
            {
                ShowTweets.Add(tweet);
            }
        }

        protected IEnumerable<Tweet> Filter(IEnumerable<Tweet> stats)
        {
            if (Predicates.Count() == 0)
            {
                return stats;
            }

            if (IsOrSearch.Value)
            {
                return Predicates.SelectMany(predicate => stats.Where(predicate))
                                 .Distinct();
            }
            else
            {
                var ret = Enumerable.Empty<Tweet>().Concat(stats);
                foreach (var predicate in Predicates)
                {
                    ret = ret.Where(predicate);
                }
                return ret;
            }
        }

        public void OnCheckFilterLinkAction()
        {
            var isFiltered = !IsFilteredLink.Value;
            IsFilteredLink.Value = isFiltered;
            ChangePredicates(isFiltered, FilterLink);
        }

        public void OnCheckFilterImagesAction()
        {
            var isFiltered = !IsFilteredImages.Value;
            IsFilteredImages.Value = isFiltered;
            ChangePredicates(isFiltered, FilterImages);
        }

        public void OnCheckFilterVideosAction()
        {
            var isFiltered = !IsFilteredVideos.Value;
            IsFilteredVideos.Value = isFiltered;
            ChangePredicates(isFiltered, FilterVideos);
        }

        public void OnCheckFilterRetweetedAction()
        {
            var isFiltered = !IsFilteredRetweeted.Value;
            IsFilteredRetweeted.Value = isFiltered;
            ChangePredicates(isFiltered, FilterRetweeted);
        }

        private void ChangePredicates(bool isFiltered, Func<Tweet, bool> func)
        {
            if (isFiltered)
            {
                Predicates.Add(func);
            }
            else
            {
                Predicates.Remove(func);
            }
            if (IsFilteredImages.Value || IsFilteredVideos.Value)
            {
                NormalGridVisibility.Value = Visibility.Collapsed;
                MediaGridVisibility.Value = Visibility.Visible;
            }
            else
            {
                MediaGridVisibility.Value = Visibility.Collapsed;
                NormalGridVisibility.Value = Visibility.Visible;
            }
            Reflesh(Filter(Tweets));
            RefleshMedias(medias);
        }

        public void OnClickOrSearchAction()
        {
            IsOrSearch.Value = true;
            if (Predicates.Count != 0)
            {
                Reflesh(Filter(Tweets));
            }
        }

        public void OnClickAndSearchAction()
        {
            IsOrSearch.Value = false;
            if (Predicates.Count != 0)
            {
                Reflesh(Filter(Tweets));
            }
        }

        private IEnumerable<Media> FilterMedias(IEnumerable<Media> medias) =>
            medias.Where(m => ShowTweets.Any(t => t.Status.Id == m.Id));
        
        private void RefleshMedias(IEnumerable<Media> medias)
        {
            Medias.Clear();
            foreach (var media in FilterMedias(medias))
            {
                Medias.Add(media);
            }
        }
        
        private void AddMedias(IEnumerable<Media> medias)
        {
            foreach (var media in FilterMedias(medias))
            {
                Medias.Add(media);
            }
        }


        public abstract void ReloadPast();


        public bool IsCheckedAndSearchAction()
        {
            return !IsOrSearch.Value;
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}
