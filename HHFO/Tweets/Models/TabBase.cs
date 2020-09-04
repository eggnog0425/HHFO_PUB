using CoreTweet;
using CoreTweet.Core;
using HHFO.Models.Logic.EventAggregator.Tweet;
using ImTools;
using NLog.Filters;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
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
using Unity;

namespace HHFO.Models
{
    public abstract class TabBase : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; }

        public long Id { get; protected set; }
        public string Name { get; protected set; }

        // チェックボックス・ラジオボタンの状態
        public ReactiveProperty<bool> IsFilteredLink { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsFilteredImages { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsFilteredVideos { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsFilteredRetweeted { get; private set; } = new ReactiveProperty<bool>(false);
        public ReadOnlyReactiveProperty<bool> IsOrSearch { get; }

        public List<Tweet> SelectedTweets { get; set; } = new List<Tweet>();
        public Tweets Tweets { get; } = new Tweets();


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

        [Dependency]
        protected ITweetPublisher TweetPublisher { get; set; }

        /// <summary>
        /// 自動更新用のタイマー
        /// </summary>
        private DispatcherTimer Timer { get; set; } = new DispatcherTimer(DispatcherPriority.Normal);

        // 表示形式の切替
        public ReactiveProperty<Visibility> NormalGridVisibility { get; } = new ReactiveProperty<Visibility>(Visibility.Visible);
        public ReactiveProperty<Visibility> MediaGridVisibility { get; } = new ReactiveProperty<Visibility>(Visibility.Collapsed);

        private Func<Tweet, bool> FilterLink = tweet => tweet.HasLinks;
        private Func<Tweet, bool> FilterImages = tweet => tweet.Media?[0].Type == "photo" || tweet.Media?[0].Type == "animated_gif";
        private Func<Tweet, bool> FilterVideos = tweet => tweet.Media?[0].Type == "video";
        private Func<Tweet, bool> FilterRetweeted = tweet => tweet.IsRetweetedTweet || tweet.QuotedTweet != null;

        protected TabBase()
        {
            Disposable = new CompositeDisposable();

            Timer.Interval = TimeSpan.FromSeconds(10.0);
            Timer.Tick += async (s, e) => await FetchTweetsAsync();
            Timer.Start();

            IsOrSearch = Tweets.IsOrSearch.ToReadOnlyReactiveProperty();
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

            OnCheckFilterLink.Subscribe(async _ => await OnCheckFilterLinkAction())
                .AddTo(Disposable);
            OnCheckFilterImages.Subscribe(async _ => await OnCheckFilterImagesAction())
                .AddTo(Disposable);
            OnCheckFilterVideos.Subscribe(async _ => await OnCheckFilterVideosAction())
                .AddTo(Disposable);
            OnCheckFilterRetweeted.Subscribe(async _ => await OnCheckFilterRetweetedAction())
                .AddTo(Disposable);
            OnClickOrSearch.Subscribe(_ => OnClickOrSearchAction())
                .AddTo(Disposable);
            OnClickAndSearch.Subscribe(_ => OnClickAndSearchAction())
                .AddTo(Disposable);
            SendReply.Subscribe(_ => SendReplyAction())
                .AddTo(Disposable);
            SelectionChange.Subscribe(e => SelectionChangeAction(e))
                .AddTo(Disposable);
            // SaveImages.Subscribe(e => SaveImagesAction(e))
            //    .AddTo(Disposable);
        }

        // TODO Modelに移管
        /*
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
            var img = new Bitmap(fileName);
            var status = Tweets.FirstOrDefault(t => t.Status.Id == media.Id).Status;
            var author = new  List<>
            
        }
    */

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

        protected abstract Task FetchTweetsAsync();


        public async Task OnCheckFilterLinkAction()
        {
            var isFiltered = !IsFilteredLink.Value;
            IsFilteredLink.Value = isFiltered;
            await ChangePredicates(isFiltered, FilterLink);
        }

        public async Task OnCheckFilterImagesAction()
        {
            var isFiltered = !IsFilteredImages.Value;
            IsFilteredImages.Value = isFiltered;
            await ChangePredicates(isFiltered, FilterImages);
        }

        public async Task OnCheckFilterVideosAction()
        {
            var isFiltered = !IsFilteredVideos.Value;
            IsFilteredVideos.Value = isFiltered;
            await ChangePredicates(isFiltered, FilterVideos);
        }

        public async Task OnCheckFilterRetweetedAction()
        {
            var isFiltered = !IsFilteredRetweeted.Value;
            IsFilteredRetweeted.Value = isFiltered;
            await ChangePredicates(isFiltered, FilterRetweeted);
        }

        private async Task ChangePredicates(bool isFiltered, Func<Tweet, bool> func)
        {
            if (isFiltered)
            {
                await Tweets.AddPredicates(func);
            }
            else
            {
                await Tweets.RemovePredicates(func);
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
            Tweets.IsOrSearch.Value = true;
        }

        public void OnClickAndSearchAction()
        {
            Tweets.IsOrSearch.Value = false;
        }

        public abstract Task ReloadPastAsync();


        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}
