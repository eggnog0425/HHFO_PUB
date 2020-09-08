using Prism.Events;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using Unity;
using HHFO.Models;
using Reactive.Bindings;

namespace HHFO.Models
{
    public class ListPublisher: IListPublisher
    {

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        public ReactivePropertySlim<long> Id { get; set; } = new ReactivePropertySlim<long>();
        public void Publish()
        {
            var list = new TwittertListId();
            list.Id.Value = this.Id.Value;
            this.EventAggregator
                .GetEvent<ListEvent>()
                .Publish(list);
        }
    }
}
