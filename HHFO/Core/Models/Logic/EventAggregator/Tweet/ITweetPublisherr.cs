using CoreTweet;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweet
{
    public interface ITweetPublisher
    {
        public long TweetId { get; set; }
        public string UserScreenName { get; set; }
        public string InReplyToScreenName { get; set; }
        public HashtagEntity[] HashTags { get; set;}
        void Publish();
    }
}
