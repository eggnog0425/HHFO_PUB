using HHFO.Models.Logic.EventAggregator.Tweets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HHFO.Models
{
    public class TabFactory : ITabFactory
    {

        public async Task<TabList> CreateListTab(long id, int surrogateKey)
        {
            return await TabList.Create(id, surrogateKey);
        }
    }
}
