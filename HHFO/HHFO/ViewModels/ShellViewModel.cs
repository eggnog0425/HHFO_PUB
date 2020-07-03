using DryIoc.Messages;
using HHFO.Core;
using HHFO.Core.Common;
using HHFO.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using System.Windows.Forms;
using Unity;

namespace HHFO.ViewModels
{
    class ShellViewModel : BindableBase
    {
        [Dependency]
        IAuth auth { get; set; }
        public string Title { get; } = new SettingUtils().getCommonSetting().Title;

        if (auth.)
    }

}
    