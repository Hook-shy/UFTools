using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UFTools.Common
{
    public static class ServiceHelper
    {
        public static bool RestartService(ServiceController service)
        {
            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"{ex.Message}\n重启服务{service.ServiceName}");
                return false;
            }
        }

        public static List<ServiceController> GetLocalSqlServices() => ServiceController.GetServices().Where(s => s.Status == ServiceControllerStatus.Running && new Regex(@"SQL Server \(.*\)", RegexOptions.IgnoreCase).IsMatch(s.DisplayName)).ToList();
    }
}
