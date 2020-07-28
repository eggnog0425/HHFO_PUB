using HHFO.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace HHFO.Tweets
{
    public class TweetsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("MainSpace", typeof(HHFO.Views.Tweets));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}