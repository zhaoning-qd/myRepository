using CommonTools;
using Entities;
using IDataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace Business
{
    class BusinessHelper
    {
        /// <summary>
        /// 设置余额
        /// </summary>
        /// <returns></returns>
        public static int  SetYe()
        {
            return 200000;
        }

        /// <summary>
        /// 获取DB2连接
        /// </summary>
        /// <returns></returns>
        public static IDB2Operation GetDb2Connection()
        {
            string assemblyName = "DataAccess";
            string namespaceName = "DataAccess";
            string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
            IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

            return iDB2Operation;
        }

        /// <summary>
        /// 查询zbmxz中某账号的笔数
        /// </summary>
        /// <returns></returns>
        public static int GetCountByZh(ZbmxzEntity zbmxz)
        {
            string assemblyName = "DataAccess";
            string namespaceName = "DataAccess";
            string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
            IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

            return iDB2Operation.ExecuteCountQuery(zbmxz.ToCountStringByZh());
        }

        /// <summary>
        /// 执行更新或插入命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static  bool ExecuteUpdateCmd(string cmd)
        {
            string assemblyName = "DataAccess";
            string namespaceName = "DataAccess";
            string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
            IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

            return iDB2Operation.ExecuteDB2Update(cmd);

        }

        /// <summary>
        /// 查询zbfhz中某账号的笔数
        /// </summary>
        /// <returns></returns>
        public static int GetCountByZh(ZbfhzEntity zbfhz)
        {
            string assemblyName = "DataAccess";
            string namespaceName = "DataAccess";
            string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
            IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

            return iDB2Operation.ExecuteCountQuery(zbfhz.ToCountStringByZh());
        }

        /// <summary>
        ///更新zbfhz和zbmxz
        /// </summary>
        /// <param name="lineArray"></param>
        /// <returns></returns>
        public static bool UpateZbfhzAndZbmxz(ZbmxzEntity zbmxz, ZbfhzEntity zbfhz)
        {
            //账表明细账
            bool zt1, zt2;
            int iBs = GetCountByZh(zbmxz);
            if (iBs == -1)
            {
                Console.WriteLine("查询账号{0}对应的笔数出错", zbmxz.Zh);
                return false;
            }
            else
            {
                string cmd = zbmxz.ToInsertString();
                if (ExecuteUpdateCmd(cmd))
                {
                    Console.WriteLine("插入zbmxz成功");
                    zt1 = true;
                }
                else
                {
                    Console.WriteLine("插入zbmxz失败");
                    zt1 = false;
                }
            }

            //账表分户账
            int num = GetCountByZh(zbfhz);

            if (num == -1)
            {
                Console.WriteLine("查询账号{0}是否存在时出错", zbfhz.Yhzh);
                return false;
            }
            if (num == 0)
            {
                if (ExecuteUpdateCmd(zbfhz.ToInsertString()))
                {
                    Console.WriteLine("插入zbfhz成功");
                    zt2 = true;
                }
                else
                {
                    Console.WriteLine("插入zbfhz失败");
                    zt2 = false;
                }
            }
            else
            {
                if (ExecuteUpdateCmd(zbfhz.ToUpdateString()))
                {
                    Console.WriteLine("更新zbfhz成功");
                    zt2 = true;
                }
                else
                {
                    Console.WriteLine("更新zbfhz失败");
                    zt2 = false;
                }
            }

            return (zt1 && zt2);
        }

    }
}
