using FroAsyncDB.BO;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FroAsyncDB
{
    public class UpdateManager
    {
        public static void SetAllCommandTimeouts(object adapter, int timeout)
        {
            var commands = adapter.GetType().InvokeMember("CommandCollection",
                    System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                    null, adapter, new object[0]);
            var sqlCommand = (System.Data.IDbCommand[])commands;
            foreach (var cmd in sqlCommand)
            {
                cmd.CommandTimeout = timeout;
            }
        }
        public static DataTable FillTable(string ConString, string CommandText)
        {
            SqlDataAdapter adp = new SqlDataAdapter(CommandText, ConString);
            adp.SelectCommand.CommandTimeout = 0;
            DataTable dt = new DataTable();
            adp.Fill(dt);
            adp.Dispose();
            return dt;
        }
        public static bool UpdateItem(update_op_config item)
        {
            string cols = item.src_cols + (item.update_op_dyn.Count == 0 ? string.Empty : $",{string.Join(",", (from q in item.update_op_dyn orderby q.src_order select q.src_col_name.Trim()))}");
            string filter = item.src_filter == null ? string.Empty : string.Format($"WHERE {item.src_filter}", (from q in item.update_op_dyn orderby q.src_order select q.src_col_val).ToArray());
            string orderby = item.update_op_dyn.Count == 0 ? string.Empty : $"ORDER BY {string.Join(",", (from q in item.update_op_dyn orderby q.src_order select $"{q.src_col_name.Trim()} ASC"))}";
            string selectString = $"SELECT {cols} FROM {item.src_tbl_name} {filter} {orderby}";
            //LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Debug, $"SELECT : {selectString}", typeof(UpdateManager));
            DataTable data = FillTable(item.update_con_str.con_str, selectString);
            if (data.Rows.Count ==
                0)
                return true;
            if (UpdateBulk(item, data))
            {
                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Log, $"{item.dst_tbl_name} - Update successfull ({data.Rows.Count})", typeof(UpdateManager));
                data.Clear();
                data.Dispose();
                return true;
            }
            else
            {
                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Error, $"{item.dst_tbl_name} Update faild", typeof(UpdateManager));
            }
            data.Clear();
            data.Dispose();
            return false;
        }
        private static bool UpdateBulk(update_op_config item, DataTable BulkTable)
        {
            bool outPut = false;
            DateTime dtStart = DateTime.Now;
            SqlConnection connection = new SqlConnection(item.update_con_str1.con_str);
            SqlCommand command = new SqlCommand("", connection) { CommandTimeout = 0 };
            try
            {
                connection.Open();
                //Create tmp table
                string BulkTableName = string.Format("IMP{0}{1}{2}{3}{4}{5}{6}", dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, dtStart.Minute, dtStart.Second, dtStart.Millisecond);
                command.CommandText = $"SELECT * INTO {BulkTableName} FROM {item.dst_tbl_name} WHERE 1 = 0;";
                command.ExecuteNonQuery();
                //Insert into tmp table
                SqlBulkCopy bulkCopy = new SqlBulkCopy(item.update_con_str1.con_str);
                bulkCopy.BulkCopyTimeout = 0;
                bulkCopy.ColumnMappings.Clear();
                string[] src_col = item.src_cols.Split(',');
                string[] dst_col = item.dst_cols.Split(',');
                for (int i = 0; i < src_col.Length; i++)
                    bulkCopy.ColumnMappings.Add(src_col[i].Trim(), dst_col[i].Trim());
                bulkCopy.DestinationTableName = BulkTableName;
                bulkCopy.BatchSize = BulkTable.Rows.Count;
                bulkCopy.WriteToServer(BulkTable);
                //clear data if required
                if (item.clear_require == true)
                {
                    command.CommandText = $"DELETE FROM {item.dst_tbl_name}";
                    command.ExecuteNonQuery();
                }
                //Merage tmp into distnation table
                string convStr = string.Join(" , ", (from q in item.update_op_conv select $"Target.{q.col_name.Trim()} = {string.Format(q.conversion.Trim(), "Source." + q.col_name.Trim())}"));
                string matchStr = string.Join(" AND ", (from q in item.update_op_key select $"Target.{q.dst_col_name.Trim()} = Source.{q.src_col_name.Trim()}"));
                string dst_cols = string.Join(",", (from q in item.update_op_conv select q.col_name.Trim()));
                string src_cols = string.Join(",", (from q in item.update_op_conv select string.Format(q.conversion.Trim(), "Source." + q.col_name.Trim())));
                command.CommandText = $"merge into {item.dst_tbl_name} as Target using {BulkTableName} AS Source ON {matchStr} when matched then UPDATE SET {convStr} " +
                    $"when not matched then INSERT ({dst_cols}) VALUES ({src_cols});";
                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Debug, $"MERGE : {command.CommandText}", typeof(UpdateManager));
                command.ExecuteNonQuery();
                //Update Dyn Fields
                DataRow maxRow = BulkTable.Select("1 = 1", string.Join(",", (from q in item.update_op_dyn orderby q.src_order select $"{q.src_col_name} DESC")))[0];
                foreach (update_op_dyn dyn in item.update_op_dyn)
                    dyn.src_col_val = maxRow[dyn.src_col_name].ToString();

                command.CommandText = string.Format(@"DROP TABLE {0}", BulkTableName);
                command.ExecuteNonQuery();

                connection.Close();

                item.update_op_log.Add(new update_op_log() { log_date = DateTime.Now, op_id = item.op_id, log_result = Types.LogResult.Succ.ToString(), eff_count = BulkTable.Rows.Count });
                outPut = true;
            }
            catch (SqlException ex)
            {
                item.update_op_log.Add(new update_op_log() { log_date = DateTime.Now, op_id = item.op_id, log_result = Types.LogResult.Erro.ToString(), log_msg = ex.StackTrace });
                LogsManager.DefaultInstance.LogMsg(LogsManager.LogType.Error, ex.Message, typeof(UpdateManager));
                command.Dispose();
                connection.Dispose();
            }
            return outPut;
        }

    }
}
