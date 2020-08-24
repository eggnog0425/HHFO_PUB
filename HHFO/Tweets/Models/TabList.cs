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
using HHFO.Models.Logic.Common;

namespace HHFO.Models
{
    public class TabList: Tab
    {
        public TabList(long id): base()
        {
            Id = id;
            Name = Token.Lists.Show(list_id => id, tweet_mode => "extended").Name;
            FetchTweets();
        }

        protected override void FetchTweets()
        {
            var token = Authorization.GetToken();
            var newTweets = Token.Lists.Statuses(list_id => Id, tweet_mode => "extended").AsEnumerable();
            var comparer = new StatusComparer();
            FilteredTweet.Concat(newTweets.Except(FilteredTweet, comparer));
            AddShow(newTweets.Except(showTweets, comparer));
        }
    }
}
