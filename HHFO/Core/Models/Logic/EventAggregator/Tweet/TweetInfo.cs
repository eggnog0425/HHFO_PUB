using CoreTweet;
using System;
using System.Collections.Immutable;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweet
{
    public class TweetInfo
    {
        public long TweetId { get; set; }
        public string UserScreenName { get; set; }
        public string InReplyToScreenName { get; set; }
        public HashtagEntity[] HashTags { get; set; }
    }
}
