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
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        private static readonly string ConnectionString = @"Data Source=E:\workspace\git\VideoAnnotation\VideoAnnotation\DB\videoinfo.sqlite;Version=3;Pooling=True;Max Pool Size=100;";
        /// <summary>
        /// 获取一个打开的数据库链接
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 保存文件信息
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static int SaveFiles(IEnumerable<dynamic> files)
        {

            try
            {
                var result = 0;
                using (var conn = GetOpenConnection())
                {
                    var sqlCommand = "insert into files(id,file_name,file_full_name,file_hash_code) values(@ID,@FileName,@FileFullName,\"\");";
                    foreach (dynamic file in files)
                    {
                        var tran = conn.BeginTransaction();
                        var paras = new SQLiteParameter[] 
                        {
                            new SQLiteParameter("ID",file.id),
                            new SQLiteParameter("FileName",file.fileName),
                            new SQLiteParameter("FileFullName",file.fileFullName)
                        };
                        var command = new SQLiteCommand(sqlCommand, conn, tran);
                        command.Parameters.AddRange(paras);
                        try
                        {
                            result += command.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("打开数据库连接失败，文件信息保存失败", ex);
            }
        }
        /// <summary>
        /// 获取还没有计算Hash的文件记录
        /// </summary>
        /// <returns></returns>
        public static dynamic GetNoHashFile()
        {
            var sql = "select id,file_full_name from files where (file_hash_code is null or file_hash_code = '')  and useable = 1 limit 0,1";
            try
            {
                using (var conn = GetOpenConnection())
                {
                    var command = new SQLiteCommand(sql, conn);
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        dynamic result = new ExpandoObject();
                        result.id = reader.GetString(0);
                        result.file_full_name = reader.GetString(1);
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("更新文件Hash值失败", ex);
            }
        }
        /// <summary>
        /// 更新文件Hash值
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="hashCode">Hash值</param>
        /// <returns></returns>
        public static bool UpdateFileHash(string fileId, string hashCode)
        {
            var sql = "update files set file_hash_code = @Code where id = @ID";
            try
            {
                using (var conn = GetOpenConnection())
                {
                    var command = new SQLiteCommand(sql, conn);
                    command.Parameters.AddRange(new SQLiteParameter[] 
                    {
                        new SQLiteParameter("ID",fileId),
                        new SQLiteParameter("Code",hashCode)
                    });
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("更新文件Hash值失败", ex);
            }
        }
        /// <summary>
        /// 设置文件状态为不可用
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="useable"></param>
        /// <returns></returns>
        public static bool UpdateFileUseable(string fileId, bool useable)
        {
            var sql = "update files set useable = @Useable where id = @ID";
            try
            {
                using (var conn = GetOpenConnection())
                {
                    var command = new SQLiteCommand(sql, conn);
                    command.Parameters.Add(new SQLiteParameter[] 
                    {
                        new SQLiteParameter("ID",fileId),
                        new SQLiteParameter("Useable",useable)
                    });
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("更新文件Useable值失败", ex);
            }
        }

        public static bool AddAnnotation(string fileId, float position, string annotation)
        {
            var id = Guid.NewGuid().ToString().Replace("-", "");
            var sql = "insert into annotations(id,file_id,position,annotation) values(@ID,@FileId,@Position,@Annotation)";
            try
            {
                using (var conn = GetOpenConnection())
                {
                    var cmd = new SQLiteCommand(sql, conn);
                    cmd.Parameters.AddRange(new SQLiteParameter[] 
                {
                    new SQLiteParameter("ID",id),
                    new SQLiteParameter("FileId",fileId),
                    new SQLiteParameter("Position",position),
                    new SQLiteParameter("Annotation",annotation)
                });
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("保存注解失败", ex);
            }
        }
    }
}
