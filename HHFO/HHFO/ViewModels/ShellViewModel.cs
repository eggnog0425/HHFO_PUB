using DryIoc.Messages;
using HHFO.Core;
using HHFO.Core.Common;
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
using System.Windows.Forms;
using Unity;
using CoreTweet;
using CoreTweet.Rest;
using System.Linq;

namespace HHFO.ViewModels
{
    class ShellViewModel : BindableBase, IDisposable
    {

        private CompositeDisposable Disposable { get; }

        public ReactiveCommand clickListButton { get; } = new ReactiveCommand();
        public string Title { get; } = new SettingUtils().getCommonSetting().Title;
        public ReactiveCommand OnLoaded { get; }

        public ShellViewModel(IRegionManager regionManager) 
        {
            this.Disposable = new CompositeDisposable();
            this.OnLoaded = new ReactiveCommand()
                .AddTo(this.Disposable);
            this.OnLoaded.Subscribe(_ =>
            {
            }
            );
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }

        private bool Authed()
        {
            var account = new SettingUtils().getUserSetting().UserAccounts;
            if(account == null || account.Length == 0)
            {
                return false;
            }
            var defaultAccount = account.Where(a => a.DefaultAccount)
                                  .Where(a => a.Token != null && a.TokenSecret != null)
                                  .FirstOrDefault();
            return (defaultAccount != null);
        }
    }
}
