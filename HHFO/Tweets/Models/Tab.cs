﻿using CoreTweet;
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
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        protected ObservableCollection<Media> medias { get; private set; } = new ObservableCollection<Media>();
        /// <summary>
        /// mediasからShowTweetに含まれるIdの発言のみを抽出
        /// </summary>
        public ReadOnlyReactiveCollection<Media> Medias { get; private set; }

        public ObservableCollection<Func<Tweet, bool>> Predicates { get; protected set; } = new ObservableCollection<Func<Tweet, bool>>();

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

        private Func<Tweet, bool> FilterLink = tweet => (tweet.Status.Entities?.Urls?.Length ?? 0) != 0;
        private Func<Tweet, bool> FilterImages = tweet => tweet.Status.ExtendedEntities?.Media?[0].Type == "photo" || tweet.Status.ExtendedEntities?.Media?[0].Type == "animated_gif";
        private Func<Tweet, bool> FilterVideos = tweet => tweet.Status.ExtendedEntities?.Media?[0].Type == "video";
        private Func<Tweet, bool> FilterRetweeted = tweet => tweet.Status.RetweetedStatus != null;

        public Tab()
        {
            Disposable = new CompositeDisposable();
            Token = Authorization.GetToken();

            timer.Interval = TimeSpan.FromSeconds(5.0);
            timer.Tick += (s, e) => FetchTweets();
            timer.Start();

            Medias = medias.Where(m => ShowTweets.Any(t => t.Status.Id == m.Id)).ToObservable().ToReadOnlyReactiveCollection();

            this.PropertyChangedAsObservable()
                .Where(e => e.PropertyName == nameof(Tweets))
                .Subscribe(_ => Reflesh(Tweets))
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

        public bool IsCheckedAndSearchAction()
        {
            return !IsOrSearch.Value;
        }


        protected Task<Media> createMedias(IEnumerable<Tweet> tweets)
        {

        }
        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}
