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
                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Log, $"Start Update ......................", typeof(Update));


                foreach (var item in _entities.update_op_config.OrderBy(o => o.op_order))
                {
                    if (!item.enable) continue;
                    if (UpdateManager.UpdateItem(item))
                    { }
                }
                _entities.SaveChanges();

                foreach (update_cube cube in _entities.update_cube)
                {
                    Microsoft.AnalysisServices.Server server = new Microsoft.AnalysisServices.Server();
                    server.Connect(cube.connectionstring);
                    Microsoft.AnalysisServices.Database database = server.Databases.FindByName(cube.database_name);
                    if (database == null)
                        continue;
                    database.Process(Microsoft.AnalysisServices.ProcessType.ProcessFull);
                    LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Log, $"Cube {cube.database_name} Processed", typeof(Update));
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
