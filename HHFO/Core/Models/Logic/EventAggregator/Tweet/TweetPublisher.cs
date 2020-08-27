using CoreTweet;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Unity;

namespace HHFO.Models.Logic.EventAggregator.Tweet
{
    public class TweetPublisher : ITweetPublisher
    {
        public long TweetId { get; set; }
        public List<string> UserScreenNames { get; set; }

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        public void Publish()
        {

            var tweet = new TweetInfo()
            {
                TweetId = this.TweetId
              , UserScreenNames = this.UserScreenNames
            };
            this.EventAggregator
                .GetEvent<TweetEvent>()
                .Publish(tweet);
        }
    }
}
