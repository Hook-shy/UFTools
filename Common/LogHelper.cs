using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFTools.Common
{
    public class LogHelper
    {
        public static void WriteLog(string logTxt)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter($@"{Environment.CurrentDirectory}\Log.txt", true, Encoding.UTF8))
                {
                    string sign = "===================================================";
                    sw.WriteLine($"\n{sign}{DateTime.Now}{sign}\n{logTxt}");
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
