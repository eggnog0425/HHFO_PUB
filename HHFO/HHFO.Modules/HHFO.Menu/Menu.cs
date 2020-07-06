using HHFO.Menu.Models;
using HHFO.Menu.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Unity;
using Unity.Lifetime;

namespace HHFO.Menu
{
    public class Menu : IModule
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public IRegionManager RegionManager { get; set; }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new System.NotImplementedException();
        }
    }
}