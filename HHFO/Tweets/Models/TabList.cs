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
    public class TabList: Tab
    {
        public TabList(string id): base()
        {

            Id = id;
            Name = Token.Lists.Show(list_id => id, tweet_mode => "extended").Name;
            AddTweets();
            AddShow(Tweets);
        }

        protected override void AddTweets()
        {
            var token = Authorization.GetToken();
            var newTweets = Token.Lists.Statuses(list_id => Id, tweet_mode => "extended").AsEnumerable();
            Tweets.AddRange(newTweets);
            AddShow(newTweets);
        }
    }
}
