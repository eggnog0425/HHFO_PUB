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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HHFO.Models
{
    public class TabList : Tab
    {
        public TabList(long id, ITweetPublisher tweetPublisher) : base(tweetPublisher)
        {
            Id = id;
            Name = Authorization.GetToken().Lists.Show(list_id => id, tweet_mode => "extended").Name;
            _ = FetchTweetsAsync();
        }

        protected override async Task FetchTweetsAsync()
        {
            var token = Authorization.GetToken();
            try
            {
                var newTweets = Authorization.GetToken().Lists.StatusesAsync(list_id => Id, tweet_mode => "extended");
                await AddTweetsAsync(newTweets);
                RefleshShowTweets();
                AddMedias();
            }
            catch (TwitterException)
            {
                //
            }
        }

        public override async Task ReloadPastAsync()
        {
            var lastId = Tweets.Min(t => t.Id);
                             
            var token = Authorization.GetToken();

            try
            {
                var newTweets = token.Lists.StatusesAsync(list_id => Id, max_id => lastId, tweet_mode => "extended");
                await AddTweetsAsync(newTweets);
            }
            catch (TwitterException)
            {
                //
            }

        }
    }
}
