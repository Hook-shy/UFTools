using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFTools.Common
{
    public class DBHelper
    {
        public DBHelper(string connectString)
        {
            this.ConnectString = connectString;
        }

        public string ConnectString { get; set; }

        public bool TestConnect()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectString))
                {
                    conn.Open();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"数据库测试连接失败\n{ex.Message}");
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public (bool, DataSet) ExeSql(string sql, string dbName)
        {
            DataSet dt = new DataSet();
            bool success = true;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectString))
                {
                    conn.Open();
                    SqlDataAdapter myda = new SqlDataAdapter($"use [{dbName}]\n{sql}", conn);
                    myda.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"use [{dbName}]\n{sql}");
                Debug.WriteLine(ex.Message);
                LogHelper.WriteLog($"{ex.Message}\nuse [{dbName}]\n{sql}");
                success = false;
            }
            return (success, dt);
        }
    }
}
