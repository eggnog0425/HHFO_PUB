using CoreTweet;
using HHFO.Models.Logic.EventAggregator.Tweet;
using ImTools;
using NLog.Filters;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
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

        /// <summary>
        /// APIの取得対象となるtweetsの全量
        /// </summary>
        protected ReactiveCollection<Tweet> Tweets { get; } = new ReactiveCollection<Tweet>();
        /// <summary>
        /// 画面に実際に表示されるtweets
        /// </summary>
        public ObservableCollection<Tweet> ShowTweets { get; private set; }
        public List<Tweet> SelectedTweets { get; set; } = new List<Tweet>();

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

        private Func<Tweet, bool> FilterLink = tweet => tweet.HasLinks;
        private Func<Tweet, bool> FilterImages = tweet => tweet.MediaType == "photo" || tweet.MediaType == "animated_gif";
        private Func<Tweet, bool> FilterVideos = tweet => tweet.MediaType == "video";
        private Func<Tweet, bool> FilterRetweeted = tweet => tweet.IsRetweetedTweet || tweet.QuotedTweet != null;

        public Tab(ITweetPublisher tweetPublisher)
        {
            Disposable = new CompositeDisposable();
            this.TweetPublisher = tweetPublisher;

            timer.Interval = TimeSpan.FromSeconds(10.0);
            timer.Tick += (s, e) => FetchTweets();
            timer.Start();

            ShowTweets = Tweets;

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

            Predicates.CollectionChangedAsObservable()
                .Subscribe(_ => OnChangedPredicate())
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
            SendReply.Subscribe(_ => SendReplyAction())
                .AddTo(Disposable);
            SelectionChange.Subscribe(e => SelectionChangeAction(e))
                .AddTo(Disposable);
            SaveImages.Subscribe(e => SaveImagesAction(e))
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

        // TODO Modelに移管
        private void SaveImagesAction(KeyEventArgs e)
        {
            Media media = (e.Source as ListBoxItem).Content as Media;
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
            
            // TODO メタデータ書き込み実装
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
            TweetPublisher.TweetId = SelectedTweets.Count == 1
                                   ? SelectedTweets[0].Id
                                   : 0;
            TweetPublisher.UserScreenNames = SelectedTweets.Select(t => t.ScreenName).ToList();
            TweetPublisher.Publish();
        }

        protected abstract void FetchTweets();

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
                SelectedTweets.Clear();
            }
            else
            {
                MediaGridVisibility.Value = Visibility.Collapsed;
                NormalGridVisibility.Value = Visibility.Visible;
                SelectedTweets.Clear();
            }
        }

        public void OnClickOrSearchAction()
        {
            IsOrSearch.Value = true;
            OnChangedPredicate();
        }


        public void OnClickAndSearchAction()
        {
            IsOrSearch.Value = false;
            OnChangedPredicate();
        }

        private void OnChangedPredicate()
        {
            ShowTweets = new ObservableCollection<Tweet>(Tweets.Where(tweet => Filter(tweet)));
        }

        public abstract void ReloadPast();

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}
