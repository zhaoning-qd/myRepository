using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDataAccess;
using System.Configuration;
using System.Data;

using IBM.Data.DB2;
using Entities.BllModels;
using Entities;


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

        /// <summary>
        /// 查询记录数查询
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public int ExecuteCountQuery(string command)
        {
            int count = 0;
            string connString = ConfigurationManager.AppSettings["DB2Connection"];
            try
            {
                DB2Connection conn = new DB2Connection(connString);
                DB2Command cmd = new DB2Command(command, conn);
                conn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();

                return count;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// 根据起始日期和结束日期查询zbmxz结果集
        /// </summary>
        /// <param name="w">业务实体</param>
        /// <returns>对象list</returns>
        public List<ZbmxzEntity> GetZbmxzByJyrq(string qsrq, string zzrq)
        {
            List<ZbmxzEntity> list = new List<ZbmxzEntity>();
            ZbmxzEntity zbmxz;
            string connString = ConfigurationManager.AppSettings["DB2Connection"];
            string cmdString = "select * from zbmxz where jyrq between '" + qsrq + "' and '" + zzrq + "'";
            try
            {
                DB2Connection conn = new DB2Connection(connString);
                DB2Command cmd = new DB2Command(cmdString, conn);
                conn.Open();
                DB2DataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    zbmxz = new ZbmxzEntity();
                    zbmxz.Bc = dr.GetInt32(1).ToString();
                    zbmxz.Zh = dr.GetString(2);
                    zbmxz.Jyrq = dr.GetDate(3).ToShortDateString();
                    zbmxz.Jysj = dr.GetDateTime(4).ToLongTimeString();
                    zbmxz.Fse = dr.GetDecimal(5).ToString();
                    zbmxz.Ye = dr.GetDecimal(6).ToString();
                    zbmxz.Yhls = dr.GetString(7);
                    zbmxz.Pjhm = dr.GetString(8);
                    zbmxz.Jdbz = dr.GetString(9);
                    zbmxz.Ywlx = dr.GetString(10);
                    zbmxz.Dfzh = dr.GetString(11);
                    zbmxz.Dfhm = dr.GetString(12);
                    zbmxz.Zxjsh = dr.GetString(13);

                    list.Add(zbmxz);
                }

                conn.Close();
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 根据机构码获取zbmxz中某机构的最大批次号
        /// </summary>
        /// <param name="jgm"></param>
        /// <returns></returns>
        public string GetMaxBacthNum(string jgm)
        {
            string pch = string.Empty;
            string connString = ConfigurationManager.AppSettings["DB2Connection"];
            string cmdString = "select max(pjhm) from (select pjhm from zbmxz where pjhm like '" + jgm + "%'" + ")";
            try
            {
                DB2Connection conn = new DB2Connection(connString);
                DB2Command cmd = new DB2Command(cmdString, conn);
                conn.Open();
                DB2DataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    pch = dr.GetString(1);
                }

                return pch;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "error";
            }
        }

        /// <summary>
        /// 根据开始批次号和结束批次号查询zbmxz
        /// </summary>
        /// <param name="spch"></param>
        /// <param name="epch"></param>
        /// <returns></returns>
        public List<ZbmxzEntity> GetZbmxzByPch(string spch, string epch)
        {
            List<ZbmxzEntity> list = new List<ZbmxzEntity>();
            ZbmxzEntity zbmxz;
            string connString = ConfigurationManager.AppSettings["DB2Connection"];
            string cmdString = "select * from zbmxz where pjhm >= '" + spch + "' and pjhm <='" + epch + "'";
            try
            {
                DB2Connection conn = new DB2Connection(connString);
                DB2Command cmd = new DB2Command(cmdString, conn);
                conn.Open();
                DB2DataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    zbmxz = new ZbmxzEntity();
                    zbmxz.Bc = dr.GetInt32(1).ToString();
                    zbmxz.Zh = dr.GetString(2);
                    zbmxz.Jyrq = dr.GetDate(3).ToShortDateString();
                    zbmxz.Jysj = dr.GetDateTime(4).ToLongTimeString();
                    zbmxz.Fse = dr.GetDecimal(5).ToString();
                    zbmxz.Ye = dr.GetDecimal(6).ToString();
                    zbmxz.Yhls = dr.GetString(7);
                    zbmxz.Pjhm = dr.GetString(8);
                    zbmxz.Jdbz = dr.GetString(9);
                    zbmxz.Ywlx = dr.GetString(10);
                    zbmxz.Dfzh = dr.GetString(11);
                    zbmxz.Dfhm = dr.GetString(12);
                    zbmxz.Zxjsh = dr.GetString(13);

                    list.Add(zbmxz);
                }

                conn.Close();
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
