using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace VideoAnnotation
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                DataHelper.CreateDatabase();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "创建数据库失败");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
