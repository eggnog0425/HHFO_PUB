using CoreTweet;
using HHFO.Models.Logic.Common;
using ImTools;
using NLog.Filters;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HHFO.Models
{
    public abstract class Tab : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; }

        public string Id { get; protected set; }
        public string Name { get; protected set; }

        // チェックボックス・ラジオボタンの状態
        public ReactiveProperty<bool> IsFilteredLink { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsFilteredImages { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsFilteredVideos { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsFilteredRetweeted { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsOrSearch { get; private set; } = new ReactiveProperty<bool>(true);

        protected Tokens Token { get; set; }
        protected IEnumerable<Status> Tweets { get; set; } = Enumerable.Empty<Status>();
        protected ObservableCollection<Status> showTweets { get; set; } = new ObservableCollection<Status>();
        public ReadOnlyReactiveCollection<Status> ShowTweets { get; protected set; }
        public ReadOnlyReactiveCollection<MediaEntity> Medias { get; protected set; }
        public ObservableCollection<Func<Status, bool>> Predicates { get; protected set; } = new ObservableCollection<Func<Status, bool>>();

        // チェックボックス・ラジオボタンエリアのコマンド
        public ReactiveCommand OnCheckFilterLink { get; }
        public ReactiveCommand OnCheckFilterImages { get; }
        public ReactiveCommand OnCheckFilterVideos { get; }
        public ReactiveCommand OnCheckFilterRetweeted { get; }
        public ReactiveCommand OnClickOrSearch { get; }
        public ReactiveCommand OnClickAndSearch { get; }
        public ReactiveCommand IsCheckedAndSearch { get; }

        /// <summary>
        /// 自動更新用のタイマー
        /// </summary>
        private DispatcherTimer timer { get; set; } = new DispatcherTimer(DispatcherPriority.Normal);

        // 表示形式の切替
        public ReactiveProperty<Visibility> NormalGridVisibility { get; } = new ReactiveProperty<Visibility>(Visibility.Visible);
        public ReactiveProperty<Visibility> MediaGridVisibility { get; } = new ReactiveProperty<Visibility>(Visibility.Collapsed);

        private Func<Status, bool> FilterLink = tweet => (tweet.Entities?.Urls?.Length ?? 0) != 0;
        private Func<Status, bool> FilterImages = tweet => tweet.ExtendedEntities?.Media[0]?.Type == "photo" || tweet.ExtendedEntities?.Media[0]?.Type == "animated_gif";
        private Func<Status, bool> FilterVideos = tweet => tweet.ExtendedEntities?.Media[0]?.Type == "video";
        private Func<Status, bool> FilterRetweeted = tweet => tweet.RetweetedStatus != null;

        public Tab()
        {
            Disposable = new CompositeDisposable();
            Token = Authorization.GetToken();
            Predicates.CollectionChanged += OnPredicatesChanged;

            timer.Interval = TimeSpan.FromSeconds(5.0);
            timer.Tick += (s, e) => FetchTweets();
            timer.Start();

            ShowTweets = showTweets.ToReadOnlyReactiveCollection()
                .AddTo(Disposable);
            Medias = showTweets.ToReadOnlyReactiveCollection(t => t.ExtendedEntities?.Media?.Aggregate((m,_) => m));
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
        }

        protected abstract void FetchTweets();

        protected void OnPredicatesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Reflesh();
        }

        protected void Clear()
        {
            showTweets.Clear();
        }

        internal void Reflesh()
        {
            Clear();
            AddShow(Tweets);
        }

        protected void AddShow(IEnumerable<Status> stats)
        {
            var filteredStats = Filter(stats);
            showTweets.AddRange(filteredStats);
        }

        protected IEnumerable<Status> Filter(IEnumerable<Status> stats)
        {
            if (Predicates.Count() == 0)
            {
                return stats;
            }

            var ret = Enumerable.Empty<Status>();
            if (IsOrSearch.Value)
            {
                foreach (var predicate in Predicates)
                {
                    ret = ret.Union(stats.Where(predicate));
                }
            }
            else
            {
                ret = ret.Concat(stats);
                foreach (var predicate in Predicates)
                {
                    ret = ret.Where(predicate);
                }
            }
            return ret;
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

        private void ChangePredicates(bool isFiltered, Func<Status, bool> func)
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
        }

        public void OnClickOrSearchAction()
        {
            IsOrSearch.Value = true;
            if (Predicates.Count != 0)
            {
                Reflesh();
            }
        }

        public void OnClickAndSearchAction()
        {
            IsOrSearch.Value = false;
            if (Predicates.Count != 0)
            {
                Reflesh();
            }
        }

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
