using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HHFO.Models
{
    public interface IListPublisher
    {
        public long Id { get; set; }
        void Publish();
    }
}
