using HHFO.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweets
{
    interface ITweetProvider
    {
        public IList<Tweet> Tweets { get; set; }
    }
}
