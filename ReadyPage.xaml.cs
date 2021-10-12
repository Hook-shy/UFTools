using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UFTools
{
    /// <summary>
    /// ReadyPage.xaml 的交互逻辑
    /// </summary>
    public partial class ReadyPage : Page
    {
        public ReadyPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private MainWindow _mainWindow;

        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("services.msc");
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

            if (HandyControl.Controls.MessageBox.Show("请确保已关闭用友U8，并做好数据备份，然后重启数据库服务\n确定要开始吗?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _mainWindow.Next();
            }
        }

        private void LastButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Last();
        }
    }
}
