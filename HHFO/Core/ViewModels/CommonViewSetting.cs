using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace HHFO.ViewModels
{
    public class CommonViewSetting: BindableBase
    {
        private Visibility menuVisibility = Visibility.Hidden;
        public Visibility MenuVisibility
        {
            get => menuVisibility;
            set 
            { 
                SetProperty(ref menuVisibility, value, nameof(MenuVisibility)); 
                RaisePropertyChanged(nameof(MenuVisibility)); 
            }
        }

    }
}
