using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using FroAsyncDB.BO;
using log4net;
using System.Timers;

namespace FroAsyncDB
{
    public static class Update
    {
        private static BO.FroAsyncDBEntities _entities = new BO.FroAsyncDBEntities();
        private static Timer _timner;
        private static int _retryOnErrorTimes = 0;

        public static void Start()
        {
            app_option update_timer_interval = _entities.app_option.Find(Types.options.update_timer_interval.ToString());
            double interval = 0;
            if (update_timer_interval != null && double.TryParse(update_timer_interval.option_value, out interval))
            {
                _timner = new Timer(interval);
            }
            else
            {
                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Error, "Option: 'update_timer_interval' not found", typeof(Update));
                Console.In.Read();
                return;
            }
            _timner.Elapsed += _timner_Elapsed;
            _timner.Start();
            _timner_Elapsed(_timner, null);
            Console.Read();
        }

        private static void _timner_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Log, $"Start updating data ......................", typeof(Update));

                foreach (var item in _entities.update_op_config.OrderBy(o => o.op_order))
                {
                    if (!item.enable) continue;
                    if (UpdateManager.UpdateItem(item) == false)
                    {
                        //reset dyn fields
                        _entities = new FroAsyncDBEntities();
                        foreach (update_op_dyn dyn in _entities.update_op_dyn)
                        {
                            if (dyn.reset_on_error == true)
                                dyn.src_col_val = "0";
                        }
                        _entities.SaveChanges();
                        LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Log, $"Reset after error ......................", typeof(Update));
                        if (_retryOnErrorTimes > 3)
                        { _retryOnErrorTimes = 0; return; }
                        _retryOnErrorTimes++;
                        _timner.Stop();
                        _timner.Start();
                        _timner_Elapsed(sender, e);
                        return;
                    }
                }
                _retryOnErrorTimes = 0;
                _entities.SaveChanges();

                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Log, $"Start executing stored procedure ......................", typeof(Update));
                SqlConnection connection = new SqlConnection(string.Empty);
                SqlCommand command = new SqlCommand(string.Empty, connection) { CommandTimeout = 0 };
                foreach (execute_sp sp in (from q in _entities.execute_sp where q.enable == true select q))
                {
                    connection.ConnectionString = sp.update_con_str.con_str;
                    connection.Open();
                    command.CommandText = sp.sp_name;
                    command.ExecuteNonQuery();
                    connection.Close();
                    LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Log, $"sp {sp.sp_name} executed", typeof(Update));
                }
                connection.Dispose(); command.Dispose();

                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Log, $"Start updating cubes ......................", typeof(Update));
                foreach (update_cube cube in (from q in _entities.update_cube where q.enable == true select q))
                {
                    Microsoft.AnalysisServices.Server server = new Microsoft.AnalysisServices.Server();
                    server.Connect(cube.connectionstring);
                    string script = cube.script;
                    string msg = string.Empty;
                    foreach (Microsoft.AnalysisServices.XmlaResult result in server.Execute(script))
                    {
                        foreach (Microsoft.AnalysisServices.XmlaMessage message in result.Messages)
                            msg += message.Description + Environment.NewLine;
                    }
                    if (msg.Trim() == string.Empty)
                        LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Log, $"Cube {cube.database_name} Processed", typeof(Update));
                    else
                        LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Error, msg, typeof(Update));

                    //Microsoft.AnalysisServices.Server server = new Microsoft.AnalysisServices.Server();
                    //server.Connect(cube.connectionstring);
                    //Microsoft.AnalysisServices.Database database = server.Databases.FindByName(cube.database_name);
                    //if (database == null)
                    //    continue;
                    //database.Process(Microsoft.AnalysisServices.ProcessType.ProcessFull);
                    //LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Log, $"Cube {cube.database_name} Processed", typeof(Update));
                }
                

                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Success, $"Update successfull ......................", typeof(Update));

            }
            catch (Exception ex)
            {
                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Error, ex.Message, typeof(Update));
            }
        }
    }
}
