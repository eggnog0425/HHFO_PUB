using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models
{
    public class ListSubscriber : BindableBase
    {
        private ReactiveProperty<long> id;
        public ReactiveProperty<long> Id
        {
            get { return id; }
            private set
            {
                this.SetProperty(ref id, value);
            }
        }


        public ListSubscriber(IEventAggregator eventAggregator)
        {
            id = new ReactiveProperty<long>();
            eventAggregator
                .GetEvent<ListEvent>()
                .Subscribe(x => {
                    Id.Value = x.Id;
                    });
        }
    }
}
