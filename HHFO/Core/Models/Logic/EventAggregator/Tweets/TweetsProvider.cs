using CoreTweet;
using HHFO.Models.Data;
using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweets
{
    public class TweetsProvider : BindableBase, ITweetsProvider
    {
        public ImmutableList<Tweet> Tweets { get; private set; }

        public bool IsReply { get; private set; } = false;

        public TweetsProvider(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TweetsEvent>()
                           .Subscribe(tuple =>
                           {
                               Tweets = tuple.Item1;
                               RaisePropertyChanged(nameof(Tweets));

                               IsReply = tuple.Item2;
                               if (IsReply)
                               {
                                   RaisePropertyChanged(nameof(IsReply));
                               }
                           }
            , ThreadOption.UIThread);
        }
    }
}