using HHFO.Models.Logic.EventAggregator.Tweets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HHFO.Models
{
    public interface ITabFactory
    {
        public Task<TabList> CreateListTab(long id, int surrogateKey);
    }
}
