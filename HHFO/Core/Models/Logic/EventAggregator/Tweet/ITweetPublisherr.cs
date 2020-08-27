using CoreTweet;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweet
{
    public interface ITweetPublisher
    {
        public long TweetId { get; set; }
        public List<string> UserScreenNames { get; set; }
        void Publish();
    }
}
