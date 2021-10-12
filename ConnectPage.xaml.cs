using System;
using System.Collections.Generic;
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
using UFTools.Common;

namespace UFTools
{
    /// <summary>
    /// ConnectPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectPage : Page
    {
        public ConnectPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private MainWindow _mainWindow;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string conStr = $"server={host.Text};database=master;integrated security=SSPI";
            if (usePwd.IsChecked == true)
            {
                conStr = $"server={host.Text};database=master;User ID={user.Text};Password={pwd.Password}";
            }
            _mainWindow.DBHelper = new DBHelper(conStr);
            if (_mainWindow.DBHelper.TestConnect())
            {
                _mainWindow.Next();
            }
            else
            {
                _mainWindow.Message("数据库连接失败", MessageType.Error);
            }
        }
    }
}
