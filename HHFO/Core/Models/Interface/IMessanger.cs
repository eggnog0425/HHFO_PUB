using System;
using System.Collections.Generic;
using System.Text;

namespace HHFO.Models.Interface
{
    public interface IMessanger<T> : IObservable<T>, IDisposable
    {
    }
}
