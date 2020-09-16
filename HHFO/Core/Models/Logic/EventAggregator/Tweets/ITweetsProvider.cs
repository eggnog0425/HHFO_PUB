using HHFO.Models.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace HHFO.Models.Logic.EventAggregator.Tweets
{
    public interface ITweetsProvider : INotifyPropertyChanged
    {
        public ObservableCollection<Tweet> Tweets { get;}
    }
}
