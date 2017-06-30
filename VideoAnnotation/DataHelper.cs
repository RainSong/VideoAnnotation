using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Dynamic;

namespace VideoAnnotation
{
    public class DataHelper
    {
        private static readonly string ConnectionString = @"Data Source=E:\workspace\git\VideoAnnotation\VideoAnnotation\DB\videoinfo.sqlite;Version=3;Pooling=True;Max Pool Size=100;";
        public static SQLiteConnection GetOpenConnection()
        {
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(ConnectionString);
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                return conn;
            }
            catch (Exception ex)
            {
                throw new Exception("打开数据库连接失败", ex);
            }
        }

        public static DataTable GetFiles()
        {
            var strCommand = "select * from files";
            try
            {
                using (SQLiteConnection conn = GetOpenConnection())
                {
                    var adapter = new SQLiteDataAdapter(strCommand, conn);
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("获取文件信息失败", ex);
            }
        }

        public static int SaveFiles(IEnumerable<dynamic> files)
        {

            try
            {
                var result = 0;
                using (var conn = GetOpenConnection())
                {
                    var sqlCommand = "insert into files(file_name,file_full_name,file_hash_code) values(@FileName,@FileFullName,\"\");";
                    foreach (dynamic file in files)
                    {
                        var tran = conn.BeginTransaction();
                        var paras = new SQLiteParameter[] 
                        {
                            new SQLiteParameter("FileName",file.fileName),
                            new SQLiteParameter("FileFullName",file.fileFullName)
                        };
                        var command = new SQLiteCommand(sqlCommand,conn,tran);
                        command.Parameters.AddRange(paras);
                        try {
                            result += command.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch(Exception ex)
                        {
                            tran.Rollback();
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("打开数据库连接失败，文件信息保存失败",ex);
            }
        }
    }
}
