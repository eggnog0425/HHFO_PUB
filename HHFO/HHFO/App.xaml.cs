using Prism.Ioc;
using HHFO.Views;
using System.Windows;
using Prism.Modularity;
using HHFO.Modules.ModuleName;
using HHFO.Services.Interfaces;
using HHFO.Services;
using NLog;
using System;
using System.Threading;
using System.Reflection;
using System.Xml;
using NLog.Config;
using System.IO;
using HHFO.Core;
using HHFO.Config;
using Prism.DryIoc;
using Prism.Mvvm;
using Unity;

namespace HHFO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private IUnityContainer Container { get; } = new UnityContainer();

        private void ApplicationStartUp(object sender, StartupEventArgs e)
        {   
            var commonSetting = HHFO.Core.CommonSetting.Value;
            var userSettingPath = commonSetting.UserSettingPath;

            try
            {
                if (!File.Exists(userSettingPath))
                {
                    var dir = Path.GetDirectoryName(userSettingPath);
                    var fileUtils = new FileUtils();
                    fileUtils.createDirectory(dir);

                    var userSetting = new UserSetting();
                    fileUtils.CreateXml<UserSetting>(userSettingPath, userSetting);
                }
            }
            catch (Exception unExpected)
            {
                logger.Error(unExpected.Message);
                logger.Error(unExpected.StackTrace);
                System.Windows.Application.Current.Shutdown(1);
            }

            new Bootstrapper().Run();
        }
    }
}