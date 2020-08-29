﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using Unity;
using HHFO.Models;

namespace HHFO.Models
{
    public class ListPublisher: IListPublisher
    {

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        public long Id { get; set; }
        public void Publish()
        {
            var list = new TwittertListId() { Id = this.Id};
            this.EventAggregator
                .GetEvent<ListEvent>()
                .Publish(list);
        }
    }
}