using CoreTweet;
using ImTools;
using NLog.Filters;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HHFO.Models
{
    public abstract class Tab : BindableBase
    {
        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public ReactiveProperty<bool> IsFilteredLink { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsSearchedAnd { get; private set; } = new ReactiveProperty<bool>(false);

        protected Tokens Token { get; set; }
        protected List<Status> Tweets { get; set; } = new List<Status>();
        protected ObservableCollection<Status> showTweets { get; set; } = new ObservableCollection<Status>();
        public ReadOnlyReactiveCollection<Status> ShowTweets { get; protected set; }
        public ObservableCollection<Func<Status, bool>> Predicates { get; protected set; } = new ObservableCollection<Func<Status, bool>>();

        private Func<Status, bool> FilterLink = tweet => tweet?.Entities?.Urls?.Length != 0;
        private Func<Status, bool> FilterImages = tweet => tweet.ExtendedEntities?.Media?[0]?.Type?.ToLower() == "photo";
        private Func<Status, bool> FilterVideos = tweet => tweet.ExtendedEntities?.Media?[0]?.Type?.ToLower() == "video";
        private Func<Status, bool> FilterRTs = tweet => tweet.IsRetweeted ?? false;

        public Tab()
        {
            Token = Authorization.GetToken();
            ShowTweets = showTweets.ToReadOnlyReactiveCollection();
            Predicates.CollectionChanged += OnPredicatesChanged;
        }

        protected void AddShow(IEnumerable<Status> stats)
        {
            var filteredStats = Filter(stats);
            showTweets.AddRange(filteredStats);
        }

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

        protected abstract void AddTweets();

        protected IEnumerable<Status> Filter(IEnumerable<Status> stats)
        {
            if (Predicates == null || Predicates.Count() == 0)
            {
                return stats;
            }

            var ret = new List<Status>();
            if (IsSearchedAnd.Value)
            {
                ret.AddRange(stats);
                foreach (var predicate in Predicates)
                {
                    ret = ret.Where(predicate).ToList();
                }
            }
            else
            {
                foreach (var predicate in Predicates)
                {
                    ret.AddRange(stats.Where(predicate));
                }
            }
            return ret;
        }

        public void OnCheckFilterLinkAction()
        {
            IsFilteredLink.Value = !IsFilteredLink.Value;
            if (IsFilteredLink.Value)
            {
                Predicates.Add(FilterLink);
            }
            else
            {
                Predicates.Remove(FilterLink);
            }
        }
    }
}
