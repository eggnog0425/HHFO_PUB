using CoreTweet;
using HHFO.Models.Logic.EventAggregator.Tweet;
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
        public TabList(long id, ITweetPublisher tweetPublisher): base(tweetPublisher)
        {
            Id = id;
            Name = Token.Lists.Show(list_id => id, tweet_mode => "extended").Name;
            FetchTweets();
        }

        protected override void FetchTweets()
        {
            var token = Authorization.GetToken();
            var newTweets = Token.Lists.Statuses(list_id => Id, tweet_mode => "extended").Select(s => new Tweet(s)).AsEnumerable();
            Tweets = newTweets.Union(Tweets).ToList();
        }

        public override void ReloadPast()
        {
            var lastId = Tweets.Select(t => t.Status.Id)
                             .Min();
            var token = Authorization.GetToken();

                try
            {
                var tweets =token.Lists.Statuses(list_id => Id, max_id => lastId, tweet_mode => "extended").AsEnumerable().Select(s => new Tweet(s));
                Tweets.AddRange(tweets);
            }
            catch (TwitterException)
            {
                //
            }
            
        }
    }
}
