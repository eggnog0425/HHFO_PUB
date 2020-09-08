using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models
{
    public class TwittertListId
    {
        public ReactivePropertySlim<long> Id { get; set; } 
        public TwittertListId()
        {
            Id = new ReactivePropertySlim<long>();
            Id.ForceNotify();
        }
    }
}
