using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// ReplacingPage.xaml 的交互逻辑
    /// </summary>
    public partial class ReplacingPage : Page
    {
        enum ProcessType
        {
            Process,
            AllProcess,
            ProcessVal,
            AllProcessVal,
            Describe,
            End
        }

        public ReplacingPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Task.Run(() =>
            {
                Start(_mainWindow.DBHelper.ConnectString, _mainWindow.ReplacePlan, _mainWindow.ACSet, (p, t) =>
                {
                    switch (t)
                    {
                        case ProcessType.Process:
                            nowP.Dispatcher.BeginInvoke(new Action<TextBlock, string>((pp, tt) => { pp.Text = tt; }), nowP, p.ToString());
                            break;
                        case ProcessType.AllProcess:
                            allP.Dispatcher.BeginInvoke(new Action<TextBlock, string>((pp, tt) => { pp.Text = tt; }), allP, p.ToString());
                            break;
                        case ProcessType.ProcessVal:
                            process.Dispatcher.BeginInvoke(new Action<CircleProgressBar, double>((pp, tt) => { pp.Value = tt; }), process, Convert.ToDouble(p));
                            break;
                        case ProcessType.AllProcessVal:
                            aProcess.Dispatcher.BeginInvoke(new Action<CircleProgressBar, double>((pp, tt) => { pp.Value = tt; }), aProcess, Convert.ToDouble(p));
                            break;
                        case ProcessType.Describe:
                            describe.Dispatcher.BeginInvoke(new Action<TextBlock, string>((pp, tt) => { pp.Text = tt; }), describe, p.ToString());
                            break;
                        case ProcessType.End:
                            _mainWindow.Dispatcher.BeginInvoke(new Action<MainWindow>(pp => { pp.ReplaceEnd = true; }), _mainWindow);
                            break;
                        default:
                            break;
                    }

                });
            });
        }

        private MainWindow _mainWindow;

        private void Start(string conStr, ReplacePlan plan, ACSet aCSet, Process p)
        {
            DBHelper dBHelper = new DBHelper(conStr);
            var x = dBHelper.ExeSql($"SELECT cDatabase FROM UA_AccountDatabase uad JOIN UA_Account ua ON uad.cAcc_Id = ua.cAcc_Id WHERE ua.cAcc_Id='{plan.OldId}' AND iYear = {aCSet.Year};", "UFSystem");
            if (!x.Item1)
            {
                return;
            }
            string dbName = x.Item2.Tables[0].AsEnumerable().Select(row => row["cDatabase"]).FirstOrDefault()?.ToString() ?? $"UFDATA_{plan.OldId}_{aCSet.Year}";
            p(0, ProcessType.AllProcessVal);
            p($"1/7 替换账套号({dbName})...", ProcessType.AllProcess);
            ReplaceId(dbName, dBHelper, plan, (pp, tt) =>
            {
                p(pp, tt);
                if (tt == ProcessType.ProcessVal)
                {
                    p(Convert.ToDouble(pp) / 100 * 5, ProcessType.AllProcessVal);
                }
            });
            p(5, ProcessType.AllProcessVal);
            p($"2/7 替换名字({dbName})...", ProcessType.AllProcess);
            ReplaceName(dbName, dBHelper, plan, (pp, tt) =>
            {
                p(pp, tt);
                if (tt == ProcessType.ProcessVal)
                {
                    p(Convert.ToDouble(pp) / 100 * (75 - 5) + 5, ProcessType.AllProcessVal);
                }
            });

            p(75, ProcessType.AllProcessVal);
            p($"3/7 替换账套号(UFMeta_{plan.OldId})...", ProcessType.AllProcess);
            ReplaceId($"UFMeta_{plan.OldId}", dBHelper, plan, (pp, tt) =>
            {
                p(pp, tt);
                if (tt == ProcessType.ProcessVal)
                {
                    p(Convert.ToDouble(pp) / 100 * (80 - 57) + 57, ProcessType.AllProcessVal);
                }
            });
            p(80, ProcessType.AllProcessVal);
            p($"4/7 替换名字(UFMeta_{plan.OldId})...", ProcessType.AllProcess);
            ReplaceName($"UFMeta_{plan.OldId}", dBHelper, plan, (pp, tt) =>
            {
                p(pp, tt);
                if (tt == ProcessType.ProcessVal)
                {
                    p(Convert.ToDouble(pp) / 100 * (85 - 80) + 80, ProcessType.AllProcessVal);
                }
            });

            p(85, ProcessType.AllProcessVal);
            p($"5/7 替换账套号(UFSystem)...", ProcessType.AllProcess);
            ReplaceId("UFSystem", dBHelper, plan, (pp, tt) =>
            {
                p(pp, tt);
                if (tt == ProcessType.ProcessVal)
                {
                    p(Convert.ToDouble(pp) / 100 * (95 - 85) + 85, ProcessType.AllProcessVal);
                }
            });
            p(95, ProcessType.AllProcessVal);
            p($"6/7 替换名字(UFSystem)...", ProcessType.AllProcess);
            ReplaceName("UFSystem", dBHelper, plan, (pp, tt) =>
            {
                p(pp, tt);
                if (tt == ProcessType.ProcessVal)
                {
                    p(Convert.ToDouble(pp) / 100 * (99 - 95) + 95, ProcessType.AllProcessVal);
                }
            });

            p(99, ProcessType.AllProcessVal);
            p($"7/7 整合数据", ProcessType.AllProcess);
            var CONSTRAINT = dBHelper.ExeSql(@"SELECT [name] FROM sysobjects WHERE xtype = 'PK' AND parent_obj = (SELECT object_id FROM sys.tables WHERE [name] = 'UA_Account');
                                               SELECT [name] FROM sysobjects WHERE xtype = 'F' AND parent_obj = (SELECT object_id FROM sys.tables WHERE [name] = 'UA_Account_sub') AND [name] LIKE '%cAcc%';
                                               SELECT [name] FROM sysobjects WHERE xtype = 'F' AND parent_obj = (SELECT object_id FROM sys.tables WHERE [name] = 'UA_Log') AND [name] LIKE '%cAcc%';
                                               SELECT [name] FROM sysobjects WHERE xtype = 'F' AND parent_obj = (SELECT object_id FROM sys.tables WHERE [name] = 'UA_Period') AND [name] LIKE '%cAcc%';", "UFSystem");
            if (CONSTRAINT.Item1 && CONSTRAINT.Item2.Tables.Count == 4)
            {
                var ca = CONSTRAINT.Item2.Tables[0].AsEnumerable().Select(row => row["name"]).FirstOrDefault();
                var cas = CONSTRAINT.Item2.Tables[1].AsEnumerable().Select(row => row["name"]).FirstOrDefault();
                var cl = CONSTRAINT.Item2.Tables[2].AsEnumerable().Select(row => row["name"]).FirstOrDefault();
                var cp = CONSTRAINT.Item2.Tables[3].AsEnumerable().Select(row => row["name"]).FirstOrDefault();
                dBHelper.ExeSql($@"ALTER TABLE UA_Account_sub DROP CONSTRAINT {cas}
                                   ALTER TABLE UA_Log DROP CONSTRAINT {cl}
                                   ALTER TABLE UA_Period DROP CONSTRAINT {cp}
                                   ALTER TABLE UA_Account DROP CONSTRAINT {ca}
                                   UPDATE UA_Account SET cAcc_Id = '{plan.NewId}' WHERE cAcc_Id = '{plan.OldId}';
                                   UPDATE UA_Account_sub SET cAcc_Id = '{plan.NewId}' WHERE cAcc_Id = '{plan.OldId}';
                                   UPDATE UA_Log SET cAcc_Id = '{plan.NewId}' WHERE cAcc_Id = '{plan.OldId}';
                                   UPDATE UA_Period SET cAcc_Id = '{plan.NewId}' WHERE cAcc_Id = '{plan.OldId}';
                                   ALTER TABLE UA_Account ADD CONSTRAINT {ca} PRIMARY KEY NONCLUSTERED(cAcc_Id)
                                   ALTER TABLE UA_Account_sub ADD CONSTRAINT {cas} FOREIGN KEY (cAcc_Id) REFERENCES UA_Account(cAcc_Id)
                                   ALTER TABLE UA_Log ADD CONSTRAINT {cl} FOREIGN KEY (cAcc_Id) REFERENCES UA_Account(cAcc_Id)
                                   ALTER TABLE UA_Period ADD CONSTRAINT {cp} FOREIGN KEY (cAcc_Id) REFERENCES UA_Account(cAcc_Id)", "UFSystem");
            }
            p($"重命名数据库", ProcessType.Process);
            if (!plan.OldId.Equals(plan.NewId))
            {
                dBHelper.ExeSql($@"IF EXISTS (SELECT * FROM sysdatabases WHERE name ='UFDATA_{plan.OldId}_{aCSet.Year}') DROP DATABASE UFDATA_{plan.NewId}_{aCSet.Year}", "master");
                dBHelper.ExeSql($@"IF EXISTS (SELECT * FROM sysdatabases WHERE name ='UFMeta_{plan.OldId}') DROP DATABASE UFMeta_{plan.NewId}", "master");
            }
            string sql = $"UPDATE UA_AccountDatabase_Ex SET cDatabase = 'UFDATA_{plan.NewId}_{aCSet.Year}' WHERE cDatabase = 'UFDATA_{plan.OldId}_{aCSet.Year}'";
            p($"[执行] {sql}...", ProcessType.Describe);
            dBHelper.ExeSql(sql, dbName);
            sql = $"UPDATE UA_AccountDatabase SET cDatabase = 'UFDATA_{plan.NewId}_{aCSet.Year}' WHERE cDatabase = 'UFDATA_{plan.OldId}_{aCSet.Year}'";
            p($"[执行] {sql}...", ProcessType.Describe);
            dBHelper.ExeSql(sql, "UFSystem");
            sql = $"EXEC sp_renamedb N'UFDATA_{plan.OldId}_{aCSet.Year}',N'UFDATA_{plan.NewId}_{aCSet.Year}';";
            p($"[执行] {sql}...", ProcessType.Describe);
            dBHelper.ExeSql(sql, "UFSystem");
            sql = $"EXEC sp_renamedb N'UFMeta_{plan.OldId}',N'UFMeta_{plan.NewId}';";
            p($"[执行] {sql}...", ProcessType.Describe);
            dBHelper.ExeSql(sql, "UFSystem");
            p(100, ProcessType.AllProcessVal);
            p(null, ProcessType.End);
        }

        private void ReplaceName(string dbName, DBHelper dBHelper, ReplacePlan plan, Process pro)
        {
            pro($"扫描/更新({dbName})...", ProcessType.Process);
            var r1 = dBHelper.ExeSql($"SELECT distinct tbs.name tName FROM sys.columns col JOIN sys.tables tbs ON col.object_id = tbs.object_id WHERE system_type_id = 231 AND max_length > 10", dbName);
            if (!r1.Item1)
            {
                return;
            }
            List<string> tables = r1.Item2.Tables[0].AsEnumerable().Select(row => row["tName"]?.ToString()).ToList();
            long index = 0;
            long success = 0;
            tables.ForEach(table =>
            {
                pro((int)(index * 100.0 / (tables.Count - 1)), ProcessType.ProcessVal);
                string sql = $"SELECT distinct col.name cName FROM sys.columns col JOIN sys.tables tbs ON col.object_id = tbs.object_id WHERE system_type_id = 231 AND max_length > 10 AND tbs.name = '{table}'";
                pro($"[扫描] {sql}...", ProcessType.Describe);
                pro($"扫描/更新({dbName})[{index}/{success}]...", ProcessType.Process);
                var r2 = dBHelper.ExeSql(sql, dbName);
                if (!r2.Item1)
                {
                    return;
                }
                List<string> cols = r2.Item2.Tables[0].AsEnumerable().Select(row => $"{row["cName"]}").ToList();
                var r3 = dBHelper.ExeSql($"SELECT count(*) n FROM [{table}] WHERE {string.Join(" OR ", cols.Select(c => $"[{c}] = '{plan.OldName}'").ToList())};", dbName);
                if (r3.Item1 && Convert.ToInt32(r3.Item2.Tables[0].AsEnumerable().Select(row => row["n"]).FirstOrDefault()) > 0)
                {
                    cols.ForEach(col =>
                    {
                        sql = $"UPDATE {table} SET [{col}] = '{plan.NewName}' WHERE [{col}] = '{plan.OldName}';";
                        success += dBHelper.ExeSql(sql, dbName).Item1 ? 1 : 0;
                        pro($"[更新] {sql}...", ProcessType.Describe);
                    });
                }
                index++;
            });
        }

        private void ReplaceId(string dbName, DBHelper dBHelper, ReplacePlan plan, Process pro)
        {
            pro($"扫描/更新({dbName})...", ProcessType.Process);
            var r1 = dBHelper.ExeSql($"SELECT tbs.name tName FROM sys.columns col JOIN sys.tables tbs ON col.object_id = tbs.object_id WHERE col.name = 'cAcc_Id'", dbName);
            if (!r1.Item1)
            {
                return;
            }
            List<string> tables = r1.Item2.Tables[0].AsEnumerable().Select(row => row["tName"]?.ToString()).ToList();
            long index = 0;
            long success = 0;
            StringBuilder errSql = new StringBuilder();
            tables.ForEach(table =>
            {
                pro((int)(index * 100.0 / (tables.Count - 1)), ProcessType.ProcessVal);
                string sql = $"SELECT distinct col.name cName FROM sys.columns col JOIN sys.tables tbs ON col.object_id = tbs.object_id WHERE col.name = 'cAcc_Id' AND tbs.name = '{table}'";
                var r2 = dBHelper.ExeSql(sql, dbName);
                pro($"[扫描] {sql}...", ProcessType.Describe);
                pro($"扫描/更新({dbName})[{index}/{success}]...", ProcessType.Process);
                if (!r2.Item1)
                {
                    return;
                }
                sql = $"UPDATE {table} SET cAcc_Id = '{plan.NewId}' WHERE cAcc_Id = '{plan.OldId}';";
                if (!dBHelper.ExeSql(sql, dbName).Item1)
                {
                    errSql.AppendLine(sql);
                }
                else
                {
                    success++;
                }
                pro($"[更新] {sql}...", ProcessType.Describe);
                index++;
            });
            dBHelper.ExeSql(errSql.ToString(), dbName);
        }

        delegate void Process(object p, ProcessType type);
    }
}
