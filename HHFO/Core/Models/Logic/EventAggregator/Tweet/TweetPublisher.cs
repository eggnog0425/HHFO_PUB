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
        public string UserScreenName { get; set; }
        public string InReplyToScreenName { get; set; }
        public HashtagEntity[] HashTags { get; set; }

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        public void Publish()
        {

            var tweet = new TweetInfo()
            {
                TweetId = this.TweetId
              , UserScreenName = this.UserScreenName
              , InReplyToScreenName = this.InReplyToScreenName
              , HashTags = this.HashTags
            };
            this.EventAggregator
                .GetEvent<TweetEvent>()
                .Publish(tweet);
        }
    }
}
