using HHFO.Models.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweets
{
    public interface ITweetPublisher
    {
        public ConcurrentBag<Tweet> Tweets { get; set; }

        public void Publish();
    }
}
