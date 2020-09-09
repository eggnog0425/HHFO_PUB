using CoreTweet;
using HHFO.Models.Data;
using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweets
{
    public class TweetProvider: ITweetProvider
    {
        public IList<Tweet> Tweets { get; set; } = new List<Tweet>();

    }
}