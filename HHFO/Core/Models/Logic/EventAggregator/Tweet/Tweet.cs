using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweet
{
    public class Tweet
    {
        public string TweetId;
        public string ScreenName;
        public string UserId;
        public List<string> HashTag;
    }
}
