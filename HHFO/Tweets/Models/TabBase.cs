using CoreTweet;
using CoreTweet.Core;
using HHFO.Models.Logic.EventAggregator.Tweets;
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
using HHFO.Models.Data;

namespace HHFO.Models
{
    public abstract class TabBase : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; }

        public long Id { get; protected set; }
        public string Name { get; protected set; }
        public int SurrogateKey { get; protected set; }

        // チェックボックス・ラジオボタンの状態
        public ReactivePropertySlim<bool> IsFilteredLink { get; private set; } = new ReactivePropertySlim<bool>(false);
        public ReactivePropertySlim<bool> IsFilteredImages { get; private set; } = new ReactivePropertySlim<bool>(false);
        public ReactivePropertySlim<bool> IsFilteredVideos { get; private set; } = new ReactivePropertySlim<bool>(false);
        public ReactivePropertySlim<bool> IsFilteredRetweeted { get; private set; } = new ReactivePropertySlim<bool>(false);
        public ReadOnlyReactivePropertySlim<bool> IsOrSearch { get; }

        public Tweets Tweets { get; } = new Tweets();


        // チェックボックス・ラジオボタンエリアのコマンド
        public ReactiveCommand OnCheckFilterLink { get; }
        public ReactiveCommand OnCheckFilterImages { get; }
        public ReactiveCommand OnCheckFilterVideos { get; }
        public ReactiveCommand OnCheckFilterRetweeted { get; }
        public ReactiveCommand OnClickAndSearch { get; }
        public ReactiveCommand SendReply { get; }
        public ReactiveCommand<SelectionChangedEventArgs> SelectionChange { get; }

        public ReactiveCommand<KeyEventArgs> SaveImages { get; }
        public ReactiveCommand<SelectionChangedEventArgs> SelectionChangeMedia { get; }

        /// <summary>
        /// 自動更新用のタイマー
        /// </summary>
        private DispatcherTimer Timer { get; set; } = new DispatcherTimer(DispatcherPriority.Normal);

        public RateLimit Limit { get; protected set; }
        public ReactivePropertySlim<string> DispNextReloadTime { get; private set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> DispNextResetTime { get; private set; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> DispLateRemaining { get; private set; } = new ReactivePropertySlim<string>();

        // 表示形式の切替
        public ReactivePropertySlim<Visibility> NormalGridVisibility { get; } = new ReactivePropertySlim<Visibility>(Visibility.Visible);
        public ReactivePropertySlim<Visibility> MediaGridVisibility { get; } = new ReactivePropertySlim<Visibility>(Visibility.Collapsed);

        private Func<Tweet, bool> FilterLink = tweet => tweet.HasLinks;
        private Func<Tweet, bool> FilterImages = tweet => tweet.Media?[0].Type == "photo" || tweet.Media?[0].Type == "animated_gif";
        private Func<Tweet, bool> FilterVideos = tweet => tweet.Media?[0].Type == "video";
        private Func<Tweet, bool> FilterRetweeted = tweet => tweet.IsRetweetedTweet || tweet.QuotedTweet != null;
        private EventHandler OnTick;

        public delegate Task ReloadEvent<EventArgs>(object sender, EventArgs e);

        public event ReloadEvent<EventArgs> Reloaded;

        public ITweetsPublisher TweetsPublisher;

        protected TabBase(long id, int surrogateKey, ITweetsPublisher tweetsPublisher)
        {
            Disposable = new CompositeDisposable();

            Id = id;
            SurrogateKey = surrogateKey;
            TweetsPublisher = tweetsPublisher;

            OnTick = async (s, e) => await FetchTweetsAsync();

            IsOrSearch = Tweets.IsOrSearch.ToReadOnlyReactivePropertySlim();
            OnCheckFilterLink = new ReactiveCommand()
                .AddTo(Disposable);
            OnCheckFilterImages = new ReactiveCommand()
                .AddTo(Disposable);
            OnCheckFilterVideos = new ReactiveCommand()
                .AddTo(Disposable);
            OnCheckFilterRetweeted = new ReactiveCommand()
                .AddTo(Disposable);
            OnClickAndSearch = new ReactiveCommand()
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
            foreach (Tweet tweet in e.AddedItems)
            {
                TweetsPublisher.Tweets.Add(tweet);
            }
            foreach(Tweet tweet in e.RemovedItems)
            {
                TweetsPublisher.Tweets.Remove(tweet);
            }
            TweetsPublisher.Publish(false);
        }

        private void SendReplyAction()
        {
            TweetsPublisher.Publish(true);
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


        public virtual void OnReloaded(EventArgs e = null)
        {
            Reloaded?.Invoke(this, e ?? EventArgs.Empty);
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
                TweetsPublisher.Tweets.Clear();
            }
            else
            {
                MediaGridVisibility.Value = Visibility.Collapsed;
                NormalGridVisibility.Value = Visibility.Visible;
                TweetsPublisher.Tweets.Clear();
            }
        }

        public void OnClickAndSearchAction()
        {
            lock (Tweets.IsOrSearch)
            {
                Tweets.IsOrSearch.Value = !Tweets.IsOrSearch.Value;
            }
        }

        public abstract Task ReloadPastAsync();

        public DateTimeOffset RefleshTimer(TimeSpan ts)
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer.Tick -= OnTick;
                Timer.Interval = ts;
                Timer.Tick += OnTick;
                Timer.Start();
            }
                return DateTimeOffset.Now.ToLocalTime() + ts;
        }

        public void RefleshDispApiInfo(DateTimeOffset nextReloadTime)
        {
            DispNextReloadTime.Value = nextReloadTime.ToString("G");
            DispLateRemaining.Value = Limit.Remaining.ToString();
            DispNextResetTime.Value = Limit.Reset.ToLocalTime().ToString("G");
        }

        public void Dispose()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer.Tick -= OnTick;
                Timer = null;
            }
            this.Disposable.Dispose();
        }
    }
}
