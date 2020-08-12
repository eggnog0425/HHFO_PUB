using CoreTweet;
using HHFO.Core.Models;
using HHFO.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.ObjectExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using Unity;

namespace HHFO.ViewModels
{
    public class TweetsViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; }

        private ListSubscriber ListSubscriber { get; set; }
        public ReadOnlyReactiveProperty<string> ListId { get; }
        public ObservableCollection<Tab> Tabs { get; }

        public ReactiveCommand<System.Windows.Input.MouseButtonEventArgs> hogehoge { get; }

        public TweetsViewModel(ListSubscriber ListIdSubscriber)
        {
            Disposable = new CompositeDisposable();
            this.ListSubscriber = ListIdSubscriber;
            ListId = this.ListSubscriber.Id.ToReadOnlyReactiveProperty();
            Tabs = new ObservableCollection<Tab>();

            hogehoge = new ReactiveCommand<System.Windows.Input.MouseButtonEventArgs>()
                .AddTo(Disposable);

            ListId.Subscribe(e => OpenList(e));
            //hogehoge.Subscribe(e => { MessageBox.Show(((System.Windows.Forms.ListViewItem)e.Source).Tag.GetType().ToString()); });
            hogehoge.Subscribe(e => { MessageBox.Show(e.GetType().ToString()); });
        }

        public void hogehogeAction()
        {

        }
        private void OpenList(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }
            var tab = AddTab();
            try
            {
                tab.init(id);
            }
            catch (TwitterException)
            {
                MessageBox.Show("リストの取得に失敗しました。");
                Tabs.Remove(tab);   
            }
        }

        private Tab AddTab()
        {
            var tab = new Tab();
            Tabs.Add(tab);
            return tab;
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }

}
