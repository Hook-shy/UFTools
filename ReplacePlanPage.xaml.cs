using HandyControl.Data;
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
    /// ReplacePlanPage.xaml 的交互逻辑
    /// </summary>
    public partial class ReplacePlanPage : Page
    {
        public ReplacePlanPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadData();
        }

        private MainWindow _mainWindow;

        private void LoadData()
        {
            oldId.Text = _mainWindow.ACSet.Id;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (newId.Text?.Length != 3)
            {
                _mainWindow.Message("新账套号必须为3位", MessageType.Error);
                return;
            }
            if (string.IsNullOrEmpty(oldName.Text))
            {
                _mainWindow.Message("原名字不能为空", MessageType.Error);
                return;
            }
            if (string.IsNullOrEmpty(newName.Text))
            {
                _mainWindow.Message("新名字不能为空", MessageType.Error);
                return;
            }
            _mainWindow.ReplacePlan = new ReplacePlan() { OldId = oldId.Text, NewId = newId.Text, OldName = oldName.Text, NewName = newName.Text };
            _mainWindow.Next();
        }

        private void LastButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Last();
        }
    }

    public class ReplacePlan
    {
        public string OldId { get; set; }
        public string NewId { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }
    }

}
