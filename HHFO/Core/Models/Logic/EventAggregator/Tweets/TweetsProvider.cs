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
        public ObservableCollection<Tweet> Tweets { get; private set; } = new ObservableCollection<Tweet>();
        
        public TweetsProvider(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TweetsEvent>()
                           .Subscribe(tList =>
                           {
                               if (tList.Count == 0)
                               {
                                   for (var i = Tweets.Count - 1; -1 < i; i--)
                                   {
                                       Tweets.RemoveAt(i);
                                   }
                                   return;
                               }
                               Tweets.Clear();
                               foreach(var t in tList)
                               {
                                   Tweets.Add(t);
                               }
                           }
            , ThreadOption.UIThread);
        }
    }
}