using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Configuration;

namespace VideoAnnotation
{
    public class DataHelper
    {        /// <summary>
        /// 获取一个打开的数据库链接
        /// </summary>
        /// <returns></returns>
        public static SQLiteConnection GetOpenConnection()
        {
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["sqlite"].ConnectionString);
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
        /// <summary>
        /// 添加一个注解
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="position">视频位置</param>
        /// <param name="annotation">注解</param>
        /// <param name="imgPath">截屏图片</param>
        /// <returns></returns>
        public static bool AddAnnotation(string fileId, float position, string annotation, string imgPath)
        {
            var id = Guid.NewGuid().ToString().Replace("-", "");
            var sql = "insert into annotations(id,file_id,position,annotation,img) values(@ID,@FileId,@Position,@Annotation,@Img)";
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
                    new SQLiteParameter("Annotation",annotation),
                    new SQLiteParameter("Img",imgPath)
                });
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("保存注解失败", ex);
            }
        }
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <returns></returns>
        public static bool CreateDatabase()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["sqlite"].ConnectionString;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "videoinfo.sqlite");
            if (string.IsNullOrEmpty(connectionString) || !File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Close();

                connectionString = string.Format(@"Data Source={0};Version=3;Pooling=True;Max Pool Size=100;", path);
                UpdateConfig(connectionString);
                return CreateTables();
            }
            return true;
        }
        /// <summary>
        /// 更新配置文件中数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public static void UpdateConfig(string connectionString)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            var exist = false;
            if (config.ConnectionStrings.ConnectionStrings["sqlite"] != null)
            {
                exist = true;
            }
            // 如果连接串已存在，首先删除它  
            if (exist)
            {
                config.ConnectionStrings.ConnectionStrings.Remove("sqlite");
            }
            //新建一个连接字符串实例  
            ConnectionStringSettings mySettings =
                new ConnectionStringSettings("sqlite", connectionString);
            // 将新的连接串添加到配置文件中.  
            config.ConnectionStrings.ConnectionStrings.Add(mySettings);
            // 保存对配置文件所作的更改  
            config.Save(ConfigurationSaveMode.Modified);
            // 强制重新载入配置文件的ConnectionStrings配置节  
            ConfigurationManager.RefreshSection("connectionStrings");
        }
        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <returns></returns>
        public static bool CreateTables()
        {
            var sql_files = @"CREATE TABLE [annotations](
                                [id] INT NOT NULL, 
                                [file_id] INT NOT NULL, 
                                [position] DOUBLE NOT NULL, 
                                [annotation] VARCHAR(500) NOT NULL,
                                [img] VARCHAR(500) NOT NULL);";
            var sql_annotation = @"CREATE TABLE [files](
                                        [id] VARCHAR(50) NOT NULL, 
                                        [file_name] VARCHAR(500) NOT NULL, 
                                        [file_full_name] VARCHAR(500) NOT NULL, 
                                        [file_hash_code] VARCHAR(100) NOT NULL, 
                                        [file_hash_type] VARCHAR(20) NOT NULL DEFAULT MD5, 
                                        [useable] BOOLEAN NOT NULL DEFAULT 1);";
            var sql_commands = new string[] {
                sql_files,
                sql_annotation
            };
            try
            {
                using (var conn = GetOpenConnection())
                {
                    var tran = conn.BeginTransaction();
                    try
                    {
                        foreach (var sql in sql_commands)
                        {
                            var cmd = new SQLiteCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw new Exception("创建表失败", ex);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("初始化数据库失败", ex);
            }
        }

        public static DataTable GetAnnotations(string fileId)
        {
            var sql = "select id,position,annotation,img from annotations where file_id = @FielId order by position";
            try
            {
                using (var conn = GetOpenConnection())
                {
                    var cmd = new SQLiteCommand(sql, conn);
                    cmd.Parameters.Add(new SQLiteParameter { ParameterName = "FielId",Value = fileId });
                    var adapter = new SQLiteDataAdapter(cmd);
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取注解信息失败", ex);
            }
        }
    }
}
