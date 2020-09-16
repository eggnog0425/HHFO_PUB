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
    public class TweetsPublisher : ITweetsPublisher
    {
        private IEventAggregator EventAggregator { get; set; }
        public IList<Tweet> Tweets { get; set; }

        public TweetsPublisher(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            Tweets = new List<Tweet>();
        }

        public void Publish()
        {
            var list = Tweets.ToImmutableList();
            this.EventAggregator
                .GetEvent<TweetsEvent>()
                .Publish(list);
        }
    }
}
