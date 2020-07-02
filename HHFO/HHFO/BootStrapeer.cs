using Unity;
using HHFO.Views;
using Prism.Unity;
using System.Windows;

namespace HHFO
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            // Unityコンテナを取得してShellウィンドウを生成
            return this.Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            // Shellの表示
            ((Window)this.Shell).Show();
        }
    }
}