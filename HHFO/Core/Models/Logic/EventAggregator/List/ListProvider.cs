using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models
{
    public class ListProvider : BindableBase
    {
        private ReactivePropertySlim<long> id;
        public ReactivePropertySlim<long> Id
        {
            get { return id; }
            private set
            {
                this.SetProperty(ref id, value);
            }
        }

        public ListProvider(IEventAggregator eventAggregator)
        {
            id = new ReactivePropertySlim<long>();
            eventAggregator
                .GetEvent<ListEvent>()
                .Subscribe(x => {
                    Id.Value = x.Id.Value;
                    Id.ForceNotify();
                    });
        }
    }
}
