using HHFO.Models.Data;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweets
{
    class TweetsEvent : PubSubEvent<IList<Tweet>>
    {
    }
}
