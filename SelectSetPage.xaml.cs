using System;
using System.Collections.Generic;
using System.Data;
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
    /// SelectSetPage.xaml 的交互逻辑
    /// </summary>
    public partial class SelectSetPage : Page
    {
        public SelectSetPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadData();
        }

        private MainWindow _mainWindow;

        private void LoadData()
        {
            var set = _mainWindow.DBHelper.ExeSql("select [name] from sysdatabases where [name] = 'UFSystem'", "master");
            if (!set.Item1)
            {
                _mainWindow.Message("数据库连接失败", MessageType.Error);
                return;
            }
            if(!(set.Item2.Tables?[0]?.Rows?.Count > 0))
            {
                _mainWindow.Message("数据库中没有检测到账套\n请初始化数据库", MessageType.Warning);
                return;
            }
            var items = _mainWindow.DBHelper.ExeSql("select * from UA_Account", "UFSystem").Item2.Tables[0].AsEnumerable().Select(row => new ACSet() { Id = row["cAcc_Id"]?.ToString(), Name = row["cAcc_Name"]?.ToString(), Year = row["iYear"]?.ToString(), Path = row["cAcc_Path"]?.ToString() }).ToList();
            items.ForEach(item =>
            {
                listView.Items.Add(item);
            });
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if(listView.SelectedItem == null)
            {
                return;
            }
            _mainWindow.ACSet = listView.SelectedItem as ACSet;
            _mainWindow.Next();
        }

        private void LastButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Last();
        }
    }

    public class ACSet
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public string Path { get; set; }
    }
}
