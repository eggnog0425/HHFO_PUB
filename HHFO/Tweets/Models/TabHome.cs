using CoreTweet;
using HHFO.Models.Logic.EventAggregator.Tweets;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HHFO.Models
{
    public class TabHome : TabBase
    {
        public static async Task<TabHome> Create(long id, int surrogateKey, ITweetsPublisher tweetsPublisher)
        {
            var ret = new TabHome(id, surrogateKey, tweetsPublisher);
            await ret.FetchTweetsAsync();
            var listInfo = await Authorization.GetToken().Lists.ShowAsync(list_id => id, tweet_mode => "extended");
            ret.Name = listInfo.Name;
            return ret;
        }

        private TabHome(long id, int surrogateKey, ITweetsPublisher tweetsPublisher) : base(id, surrogateKey, tweetsPublisher) { }

        protected override async Task FetchTweetsAsync()
        {
            var token = Authorization.GetToken();
            try
            {
                var newTweets = Authorization.GetToken().Statuses.HomeTimelineAsync(tweet_mode => "extended");
                Limit = await Tweets.AddTweetsAsync(newTweets);
                OnReloaded();
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
                OnReloaded();
            }
            catch (TwitterException)
            {
                //
            }

        }
    }

}
