using CoreTweet;
using HHFO.Models.Data;
using Prism.Events;
using Reactive.Bindings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Unity;

namespace HHFO.Models.Logic.EventAggregator.Tweets
{
    public class TweetPublisher : ITweetPublisher
    {
        private IEventAggregator EventAggregator { get; set; }
        public ConcurrentBag<Tweet> Tweets { get; set; }

        public TweetPublisher(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            Tweets = new ConcurrentBag<Tweet>();
        }

        public void Publish()
        {
            var list = Tweets.ToArray().ToImmutableList();
            this.EventAggregator
                .GetEvent<ListEvent>()
                .Publish(list);
        }
    }
}
