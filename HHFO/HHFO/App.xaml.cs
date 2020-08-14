using HHFO.Config;
using HHFO.Models;
using HHFO.ViewModels;
using HHFO.Views;
using NLog;
using NLog.Config;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Xml;
using Unity;
using System.Linq;

namespace HHFO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IListProvider, ListProvider>();
            containerRegistry.Register<ListSubscriber, ListSubscriber>();
            containerRegistry.Register<CommonViewSetting>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<HHFO.Menu.MenuModule>();
            moduleCatalog.AddModule<HHFO.Tweets.TweetsModule>();
            moduleCatalog.AddModule<HHFO.Tweet.TweetModule>();
        }
        private void ApplicationStartUp(object sender, StartupEventArgs e)
        {
            try
            {
                var settingUtils = new SettingUtils();
                var userSetting = settingUtils.getUserSetting();
                if (userSetting == null)
                {
                    if (!settingUtils.CreateUserSetting(out var message))
                    {
                        throw new Exception(message);
                    }
                }
                var defaultAccont = userSetting?.UserAccounts?.FirstOrDefault(ua => ua.DefaultAccount);
                if(defaultAccont != null)
                {
                    var token = defaultAccont.Token;
                    var tokenSecret = defaultAccont.TokenSecret;
                    Authorization.GetToken(token: token, tokenSecret: tokenSecret);
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception.Message);
                logger.Error(exception.StackTrace);
                MessageBox.Show("起動に失敗しました");
                System.Windows.Application.Current.Shutdown(1);
            }
        }
    }
}