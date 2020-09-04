using CoreTweet;
using HHFO.Models.Logic.Common;
using NLog;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HHFO.Models
{
    public class Media : BindableBase, IDisposable
    {
        public string MediaUrl { get; }
        public long TweetId { get; }
        public string Type { get; }

        private CompositeDisposable Disposable = new CompositeDisposable();

        public Media(long id, MediaEntity entity)
        {
            MediaUrl = entity.MediaUrlHttps;
            TweetId = id;
            Type = entity.Type;
        }


        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}
