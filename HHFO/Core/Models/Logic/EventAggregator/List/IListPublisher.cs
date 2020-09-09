using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HHFO.Models
{
    public interface IListPublisher
    {
        public ReactivePropertySlim<long> Id { get; set; }
        public void Publish();
    }
}
