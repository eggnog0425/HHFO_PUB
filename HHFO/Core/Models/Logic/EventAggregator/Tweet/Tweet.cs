using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweet
{
    public class Tweet
    {
        public long TweetId;
        public string ScreenName;
        public long UserId;
        public List<string> HashTag;
    }
}
