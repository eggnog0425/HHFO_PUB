using HHFO.Models.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweets
{
    public class TweetsEvent : PubSubEvent<(ImmutableList<Tweet>, bool)>
    {
    }
}
