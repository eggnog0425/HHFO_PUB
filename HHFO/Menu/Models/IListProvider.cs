using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HHFO.Models
{
    public interface IListProvider
    {
        public string Id { get; set; }
        void Publish();
    }
}
