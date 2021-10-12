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

namespace UFTools
{
    /// <summary>
    /// FinishPage.xaml 的交互逻辑
    /// </summary>
    public partial class FinishPage : Page
    {
        public FinishPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private MainWindow _mainWindow;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Home();
        }
    }
}
