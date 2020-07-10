using HHFO.Config;
using HHFO.Core;
using HHFO.Core.Common;
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
            containerRegistry.Register<HHFO.Models.AbstractMenu, HHFO.Models.Menu>();

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<HHFO.Menu.MenuModule>();
        }
        private void ApplicationStartUp(object sender, StartupEventArgs e)
        {
            try
            {
                var settingUtils = new SettingUtils();
                if (settingUtils.getUserSetting() == null)
                {
                    if (!settingUtils.createUserSetting(out var message))
                    {
                        throw new Exception(message);
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception.Message);
                logger.Error(exception.StackTrace);
                MessageBox.Show("花発多風雨の起動に失敗しました");
                System.Windows.Application.Current.Shutdown(1);
            }
        }
    }
}