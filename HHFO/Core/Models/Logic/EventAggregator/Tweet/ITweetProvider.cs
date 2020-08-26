using CoreTweet;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweet
{
    public interface ITweetProvider
    {
        public ReactiveProperty<TweetInfo> Tweet { get; }
    }
}
