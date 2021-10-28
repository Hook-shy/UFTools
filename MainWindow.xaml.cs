using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
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
    public enum MessageType
    {
        Success,
        Info,
        Error,
        Warning
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SwitchPage();
            dragRec.MouseLeftButtonDown += (o, e) => { DragMove(); };
        }

        public DBHelper DBHelper { get; set; }
        public ACSet ACSet { get; set; }
        public ReplacePlan ReplacePlan { get; set; }
        public bool ReplaceEnd { set { Next(); } }

        public void Next()
        {
            if (stepbar.StepIndex == stepbar.Items.Count - 1)
            {
                return;
            }
            stepbar.StepIndex += 1;
            curTitle.Text = (stepbar.Items[stepbar.StepIndex] as StepBarItem).Content?.ToString();
            SwitchPage();
        }

        public void Last()
        {
            if (stepbar.StepIndex == 0)
            {
                return;
            }
            stepbar.StepIndex -= 1;
            frame.GoBack();
        }

        public void Home()
        {
            stepbar.StepIndex = 0;
            SwitchPage();
        }

        private void SwitchPage()
        {
            switch (stepbar.StepIndex)
            {
                case 0:
                    frame.Content = new ConnectPage(this);
                    break;
                case 1:
                    frame.Content = new SelectSetPage(this);
                    break;
                case 2:
                    frame.Content = new ReplacePlanPage(this);
                    break;
                case 3:
                    frame.Content = new ReadyPage(this);
                    break;
                case 4:
                    frame.Content = new ReplacingPage(this);
                    break;
                case 5:
                    frame.Content = new FinishPage(this);
                    break;
                default:
                    break;
            }
        }

        private void ImageAwesome_MouseLeave(object sender, MouseEventArgs e)
        {
            if (HandyControl.Controls.MessageBox.Show("确定要退出吗?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void Open_Log(object sender, MouseEventArgs e)
        {
            if (File.Exists("Log.txt"))
            {
                try
                {
                    Process.Start("notepad.exe", $@"{Environment.CurrentDirectory}\Log.txt");
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog($"{ex.Message}\n打开日志");
                }
            }
        }

        public void Message(string connect, MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Success:
                    Growl.Success(connect);
                    break;
                case MessageType.Info:
                    Growl.Info(connect);
                    break;
                case MessageType.Error:
                    Growl.Error(connect);
                    break;
                case MessageType.Warning:
                    Growl.Warning(connect);
                    break;
                default:
                    break;
            }
        }
    }
}
