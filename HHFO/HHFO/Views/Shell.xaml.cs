using System.Windows;
using Prism.Mvvm;

namespace HHFO.Views
{
    /// <summary>
    /// Shell.xaml の相互作用ロジック
    /// </summary>
    public partial class Shell : MahApps.Metro.Controls.MetroWindow
    {
        public Shell()
        {
            InitializeComponent();
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }
    }
}
