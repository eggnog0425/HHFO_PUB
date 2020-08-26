using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweet
{
    public class TweetEvent:PubSubEvent<TweetInfo>
    {
    }
}
