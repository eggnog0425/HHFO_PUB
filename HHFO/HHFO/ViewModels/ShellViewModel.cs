using DryIoc.Messages;
using HHFO.Models;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Windows;
using Unity;
using CoreTweet;
using CoreTweet.Rest;
using System.Linq;
using System.Diagnostics;

namespace HHFO.ViewModels
{
    class ShellViewModel : BindableBase, IDisposable
    {

        private CompositeDisposable Disposable { get; }

        public string Title { get; } = new SettingUtils().getCommonSetting().Title;
        private Authorization Auth { get; set; }

        public ReactiveCommand OnLoaded { get; }
        public ReactiveCommand OpenBrowser { get; }
        public ReactiveCommand InitialAuth { get; }


        public ReactiveProperty<bool> OpenFlyOut { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<String> Pin { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<Visibility> PinError { get; } = new ReactiveProperty<Visibility>(Visibility.Collapsed);

        public ShellViewModel(IRegionManager regionManager) 
        {
            Disposable = new CompositeDisposable();
            OnLoaded = new ReactiveCommand()
                .AddTo(Disposable);
            OpenBrowser = new ReactiveCommand()
                .AddTo(Disposable);
            InitialAuth = new ReactiveCommand()
                .AddTo(Disposable);

            OnLoaded.Subscribe(_ =>
            {
                if (!Authorization.Authed())
                {
                    OpenFlyOut.Value = true;
                }
            }
            );

            OpenBrowser.Subscribe(_ =>
            {
                Auth = new Authorization();
                Auth.OpenAuthPage();
            }
            );

            InitialAuth.Subscribe(_ =>
            {
                if (Auth != null && Auth.InitialAuth(Pin.Value))
                {
                    OpenFlyOut.Value = false;
                }
                else
                {
                    PinError.Value = Visibility.Visible;
                }
            }
            );
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }


    }
}
