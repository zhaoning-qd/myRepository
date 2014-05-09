using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace CommonTools
{
    public static class LogHelper
    {
        /// <summary>
        /// 输出日志，记录异常;
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void WriteLogException(string str, Exception ex)
        {
            ILog log = LogManager.GetLogger(str);
            log.Error("Error:", ex);
        }

        /// <summary>
        /// 输出日志，记录自定义错误信息;
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteLogError(string str, string msg)
        {
            ILog log = LogManager.GetLogger(str);
            log.Error(msg);
        }

        /// <summary>
        /// 输出日志，记录其他信息;
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        public static void WriteLogInfo(string str, object msg)
        {
            ILog log = LogManager.GetLogger(str);
            log.Info(msg);
        }
    }
}
