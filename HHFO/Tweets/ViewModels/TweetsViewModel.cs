using ControlzEx.Standard;
using CoreTweet;
using HHFO.Models;
using ImTools;
using MahApps.Metro.Controls;
using NLog;
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
        public ReactiveProperty<bool> IsExpandedHeader { get; private set; } = new ReactiveProperty<bool>(true);

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private CompositeDisposable Disposable { get; set; }
        private ListProvider ListProvider { get; set; }
        public Tab CurrentTab { get; private set; }

        private FrameworkElement ParentElement { get; set; }
        private TabControl TabControl { get; set; }

        private ReadOnlyReactiveProperty<long> ListId { get; }
        public ObservableCollection<Tab> Tabs { get; }
        public ReactiveProperty<double> DataGridHeight { get; set; } = new ReactiveProperty<double>(0.0d);
        public ReactiveProperty<double> DataGridWidth { get; set; } = new ReactiveProperty<double>(0.0d);
        public ReactiveProperty<bool> IsOpenCheckBoxArea { get; set; } = new ReactiveProperty<bool>(true);

        public ReactiveCommand<SizeChangedEventArgs> OnSizeChanged { get; }
        public ReactiveCommand<RoutedEventArgs> OnLoaded { get; }
        public ReactiveCommand<RoutedEventArgs> OnContentLoaded { get; }
        public ReactiveCommand<SelectionChangedEventArgs> OnCurrentTabChanged { get; }
        public ReactiveCommand<System.Windows.Input.MouseButtonEventArgs> OnTabClose { get; }
        
        public TweetsViewModel(ListProvider ListIdProvider)
        {
            Disposable = new CompositeDisposable();
            this.ListProvider = ListIdProvider;
            ListId = this.ListProvider.Id.ToReadOnlyReactiveProperty();
            Tabs = new ObservableCollection<Tab>();

            OnSizeChanged = new ReactiveCommand<SizeChangedEventArgs>()
                .AddTo(Disposable);
            OnLoaded = new ReactiveCommand<RoutedEventArgs>()
                .AddTo(Disposable);
            OnContentLoaded = new ReactiveCommand<RoutedEventArgs>()
                .AddTo(Disposable);
            OnCurrentTabChanged = new ReactiveCommand<SelectionChangedEventArgs>()
                .AddTo(Disposable);
            OnTabClose = new ReactiveCommand<System.Windows.Input.MouseButtonEventArgs>()
                .AddTo(Disposable);

            OnSizeChanged.Subscribe(e => OnSizeChangedAction(e));
            ListId.Subscribe(e => OpenListTabAction(e))
                .AddTo(Disposable);
            OnLoaded.Subscribe(e => OnLoadedAction(e))
                .AddTo(Disposable);
            OnContentLoaded.Subscribe(e => OnContentLoadedAction(e))
                .AddTo(Disposable);
            OnCurrentTabChanged.Subscribe(e => OnCurrentTabChangedAction())
                .AddTo(Disposable);
            OnTabClose.Subscribe(e => OnTabCloseAction(e))
                .AddTo(Disposable);
        }

        private void OnContentLoadedAction(RoutedEventArgs e)
        {
            ChangeDataGridSize((FrameworkElement)e.OriginalSource);
        }

        private void OnTabCloseAction(MouseButtonEventArgs e)
        {
            var hoge = e.Source.GetType().ToString();
        }

        private void OnCurrentTabChangedAction()
        {
            CurrentTab = (Tab)TabControl.SelectedItem;
        }

        private void ChangeDataGridSize(FrameworkElement parent)
        {
            var otherHeight = 0.0d;
            foreach(var item in parent.GetChildObjects()) {
                if (item is FrameworkElement)
                {
                    FrameworkElement element = item as FrameworkElement;
                    if (element.Name != "NormalTweetView" && element.Name != "MediaTweetView")
                    {
                        otherHeight += element.ActualHeight;
                    }
                }
            }
            DataGridHeight.Value = parent.ActualHeight - otherHeight;
            DataGridWidth.Value = parent.ActualWidth;
        }

        private void OnSizeChangedAction(SizeChangedEventArgs e)
        {
            ChangeDataGridSize((FrameworkElement)e.OriginalSource);
        }

        private void OnLoadedAction(RoutedEventArgs e)
        {
            ParentElement = (FrameworkElement)e.OriginalSource;

            foreach (var item in ParentElement.GetChildObjects())
            {
                if (item is TabControl)
                {
                    TabControl = item as TabControl;
                    return;
                }
            }
        }

        private void OpenListTabAction (long id)
        {
            if (id == 0)
            {
                return;
            }

            try
            {
                var tab = Tabs.FirstOrDefault(t => t.Id == id);
                if (tab == null)
                {
                    tab = new TabList(id);
                    Tabs.Add(tab);
                }
                SelectTabByVM(id);
            }
            catch (TwitterException)
            {
                MessageBox.Show("リストの取得に失敗しました。");
            }
        }

        private void SelectTabByVM(long id)
        {
            var i = 0;
            foreach (Tab item in TabControl.Items)
            {
                if (item.Id == id)
                {
                    TabControl.SelectedIndex = i;
                    return;
                }
                i++;
            }
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }

}
