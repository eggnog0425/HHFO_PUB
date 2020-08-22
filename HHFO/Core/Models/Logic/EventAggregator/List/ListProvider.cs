using Prism.Events;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using Unity;
using HHFO.Models;

namespace HHFO.Models
{
    public class ListProvider: IListProvider
    {

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        public string Id { get; set; }
        public void Publish()
        {
            var list = new TwittertListId() { Id = this.Id};
            this.EventAggregator
                .GetEvent<ListEvent>()
                .Publish(list);
        }
    }
}
