using HHFO.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace HHFO.Menu
{
    public class MenuModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("MenuSpace", typeof(HHFO.Views.Menu));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}