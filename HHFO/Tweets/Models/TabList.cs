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
            Name = Authorization.GetToken().Lists.Show(list_id => id, tweet_mode => "extended").Name;
            FetchTweets();
        }

        protected override void FetchTweets()
        {
            var token = Authorization.GetToken();
            try
            {
                var newTweets = Authorization.GetToken().Lists.Statuses(list_id => Id, tweet_mode => "extended").Select(s => new Tweet(s)).AsEnumerable();
                foreach (var newTweet in newTweets)
                {
                    if (!Tweets.Any(tweet => tweet.Id == newTweet.Id))
                    {
                        Tweets.Add(newTweet);
                    }
                }
            }catch (TwitterException)
            {
                //
            }
        }

        public override void ReloadPast()
        {
            var lastId = Tweets.Select(t => t.Id)
                             .Min();
            var token = Authorization.GetToken();

                try
            {
                var newTweets =token.Lists.Statuses(list_id => Id, max_id => lastId, tweet_mode => "extended").AsEnumerable().Select(s => new Tweet(s));
                foreach(var tweet in newTweets)
                {
                    Tweets.Add(tweet);
                }
            }
            catch (TwitterException)
            {
                //
            }
            
        }
    }
}
