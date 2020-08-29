using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;

namespace HHFO.Models
{
    public class Media : BindableBase, IDisposable
    {
        public long Id { get; private set; }
        public string Type { get; private set; }
        public string MediaUrl { get; private set; }

        public CompositeDisposable Disposable;

        public ReactiveProperty<bool> IsSelected { get; set; } = new ReactiveProperty<bool>(false);

        public ReactiveCommand<RoutedEventArgs> MediaGotFocus { get; }
        public ReactiveProperty<bool> IsFocus = new ReactiveProperty<bool>(false);
        public Media(long id, string type, string mediaUrl)
        {
            Id = id;
            Type = type;
            MediaUrl = mediaUrl;

            Disposable = new CompositeDisposable();
            MediaGotFocus = new ReactiveCommand<RoutedEventArgs>()
                .AddTo(Disposable);

            MediaGotFocus.Subscribe(e => MediaGotFocusAction(e))
                .AddTo(Disposable);
        }

        private void MediaGotFocusAction(RoutedEventArgs e)
        {
            IsFocus.Value = true;
            var a = e.OriginalSource;
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}
