using CoreTweet;
using ImTools;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HHFO.Models
{
    public class Tab
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        private ObservableCollection<Status> statuses { get; set; } = new ObservableCollection<Status>();
        public ReadOnlyReactiveCollection<Status> Statuses { get; private set; }
        private DataGrid DataGrid { get; set; }
        public ReadOnlyReactiveCollection<Status> SelectedStatuses { get; private set; }

        public Tab(string id, DataGrid dataGrid)
        {
            this.Id = id;
            var token = Authorization.GetToken();
            this.Name = token.Lists.Show(list_id => id).Name;
            var statuses = token.Lists.Statuses(list_id => id);
            AddStatuses(statuses.AsEnumerable());
            Statuses = this.statuses.ToReadOnlyReactiveCollection();
            DataGrid = dataGrid;
            SelectedStatuses = DataGrid.SelectedItems
                                       .OfType<Status>()
                                       .ToObservable()
                                       .ToReadOnlyReactiveCollection();
        }

        internal void AddStatuses(IEnumerable<Status> stats)
        {
            statuses.AddRange(stats);
        }

        internal void RemoveStatuses(IEnumerable<Status> stats)
        {
            foreach (var stat in stats)
            {
                statuses.Remove(stat);
            }
        }
    }
}
