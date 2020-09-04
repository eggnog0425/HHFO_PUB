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
    public class TabList : TabBase
    {
        public static async Task<TabList> Create(long id)
        {
            var ret = new TabList(id);
            await ret.FetchTweetsAsync();
            var listInfo = await Authorization.GetToken().Lists.ShowAsync(list_id => id, tweet_mode => "extended");
            ret.Name = listInfo.Name;
            return ret;
        }
        private TabList(long id) : base()
        {
            Id = id;
        }

        protected override async Task FetchTweetsAsync()
        {
            var token = Authorization.GetToken();
            try
            {
                var newTweets = Authorization.GetToken().Lists.StatusesAsync(list_id => Id, tweet_mode => "extended");
                await Tweets.AddTweetsAsync(newTweets);
            }
            catch (TwitterException)
            {
                //
            }
        }

        public override async Task ReloadPastAsync()
        {
            var lastId = Tweets.MinId();
                             
            var token = Authorization.GetToken();

            try
            {
                var newTweets = token.Lists.StatusesAsync(list_id => Id, max_id => lastId, tweet_mode => "extended");
                await Tweets.AddTweetsAsync(newTweets);
            }
            catch (TwitterException)
            {
                //
            }

        }
    }
}
