using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDataAccess;
using System.Configuration;
using IBM.Data.DB2;

namespace DataAccess
{
    /// <summary>
    /// DB2数据库操作
    /// </summary>
    public class DB2Operation : IDB2Operation
    {
        /// <summary>
        /// 执行DB2数据库更新命令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool ExecuteDB2Update(string command)
        {
            string connString = ConfigurationManager.AppSettings["DB2Connection"];
            try
            {
                DB2Connection conn = new DB2Connection(connString);
                DB2Command cmd = new DB2Command(command, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                return true;

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
