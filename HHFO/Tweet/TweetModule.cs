using HHFO.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace HHFO.Tweet
{
    public class TweetModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("TweetSpace", typeof(HHFO.Views.Tweet));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}