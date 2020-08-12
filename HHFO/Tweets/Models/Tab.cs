using CoreTweet;
using ImTools;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace HHFO.Models
{
    public class Tab
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        private IObservable<Status> statuses { get; set; }
        public ReadOnlyReactiveCollection<Status> Statuses { get; private set; }

        internal void init(string id)
        {
            this.Id = id;
            var token = Authorization.GetToken();
            this.Name = token.Lists.Show(list_id => id).Name;
            var statuses = token.Lists.Statuses(list_id => id);
            this.statuses = statuses.ToObservable();
            Statuses = this.statuses.ToReadOnlyReactiveCollection();
        }
    }
}
