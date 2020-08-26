using CoreTweet;
using HHFO.Models.Logic.EventAggregator.Tweet;
using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace HHFO.Models
{
    public class TweetProvider : BindableBase, ITweetProvider
    {
        public ReactiveProperty<TweetInfo> Tweet { get; private set; }

        public TweetProvider(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TweetEvent>()
                           .Subscribe(tweet => this.Tweet = new ReactiveProperty<TweetInfo>(tweet));
        }
    }
}