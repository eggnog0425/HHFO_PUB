using CoreTweet;
using ImTools;
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
    public class Tab
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        private IEnumerable<Status> Tweets { get; set; }
        private ObservableCollection<Status> showTweets { get; set; } = new ObservableCollection<Status>();
        public ReadOnlyReactiveCollection<Status> ShowTweets { get; private set; }

        public Tab(string id)
        {
            this.Id = id;
            var token = Authorization.GetToken();
            this.Name = token.Lists.Show(list_id => id, tweet_mode => "extended").Name;
            Tweets = token.Lists.Statuses(list_id => id, tweet_mode => "extended").AsEnumerable();
            Add(Tweets);
            ShowTweets = this.showTweets.ToReadOnlyReactiveCollection();
        }

        private void Add(IEnumerable<Status> stats)
        {
            showTweets.AddRange(stats);
        }

        private void Clear()
        {
            showTweets.Clear();
        }
        internal void ShowAll()
        {
            Clear();
            Add(Tweets);
        }
        internal void Filter(params Func<Status, bool>[] predicates)
        {
            Clear();
            foreach (var predicate in predicates)
            {
                Add(Tweets.Where(predicate));
            }
        }
    }
}
