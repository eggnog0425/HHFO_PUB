using DryIoc.Messages;
using HHFO.Core;
using HHFO.Core.Common;
using HHFO.Models;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Reactive.Bindings;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Unity;

namespace HHFO.ViewModels
{
    class ShellViewModel : BindableBase
    {

        public ReactiveCommand clickListButton { get; } = new ReactiveCommand();
        public string Title { get; } = new SettingUtils().getCommonSetting().Title;

        public ShellViewModel(IRegionManager regionManager) 
        {
        }
    }
}
