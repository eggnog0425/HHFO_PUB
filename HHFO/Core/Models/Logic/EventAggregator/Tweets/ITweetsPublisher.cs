using HHFO.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweets
{
    public interface ITweetsPublisher
    {
        public IList<Tweet> Tweets { get; set; }

        public void Publish(bool isReply);
    }
}
