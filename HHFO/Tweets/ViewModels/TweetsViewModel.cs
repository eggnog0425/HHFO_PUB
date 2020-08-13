using ControlzEx.Standard;
using CoreTweet;
using HHFO.Core.Models;
using HHFO.Models;
using ImTools;
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
using System.Windows.Controls;
using System.Windows.Input;
using Unity;
using Status = CoreTweet.Status;

namespace HHFO.ViewModels
{
    public class TweetsViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; }

        private ListSubscriber ListSubscriber { get; set; }
        public ReadOnlyReactiveProperty<string> ListId { get; }
        public ObservableCollection<Tab> Tabs { get; }
        public Tab CurrentTab { get; set; }
        private TabControl tabControl { get; set; }

        public ReactiveCommand<SelectionChangedEventArgs> SelectTab { get; }
        public ReactiveCommand<EventArgs> SelectStatuses { get; }
        public ReactiveCommand<RoutedEventArgs> OnLoaded { get; }

        public TweetsViewModel(ListSubscriber ListIdSubscriber)
        {
            Disposable = new CompositeDisposable();
            this.ListSubscriber = ListIdSubscriber;
            ListId = this.ListSubscriber.Id.ToReadOnlyReactiveProperty();
            Tabs = new ObservableCollection<Tab>();

            SelectStatuses = new ReactiveCommand<EventArgs>()
                .AddTo(Disposable);
            SelectTab = new ReactiveCommand<SelectionChangedEventArgs>()
                .AddTo(Disposable);
            OnLoaded = new ReactiveCommand<RoutedEventArgs>()
                .AddTo(Disposable);

            OnLoaded.Subscribe(e => OnLoadedAction(e));
            ListId.Subscribe(e => OpenList(e));
            SelectStatuses.Subscribe(e => SelectStatusesAction(e));
            SelectTab.Subscribe(e => SelectTabAction(e));
        }

        private void OnLoadedAction(RoutedEventArgs e)
        {
            tabControl = (TabControl)e.Source;
        }

        private void SelectStatusesAction(EventArgs e)
        {
            e.GetType();
        }

        private void SelectTabAction(SelectionChangedEventArgs e)
        {
            if (!(e.Source is TabControl))
            {
                return;
            }
            var tabContorol = (TabControl)e.Source;
            CurrentTab = (Tab)tabContorol.SelectedItem;
        }

        private void OpenList(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }
            try
            {
                var tab = new Tab(id, (DataGrid)tabControl.SelectedContent);
                Tabs.Add(tab);
            }
            catch (TwitterException)
            {
                MessageBox.Show("リストの取得に失敗しました。");
            }
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }

}
