using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    /// <summary>
    /// 账表分户账实体
    /// </summary>
    public class ZbfhzEntity
    {
        /// <summary>
        /// 银行账号
        /// </summary>
        public string Yhzh { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public string Ye { get; set; }

        /// <summary>
        /// 笔数
        /// </summary>
        public string Bs { get; set; }

        /// <summary>
        /// 上笔日期
        /// </summary>
        public string Sbrq { get; set; }

        /// <summary>
        /// 户名
        /// </summary>
        public string Hm { get; set; }

        public ZbfhzEntity()
        {
            this.Sbrq = DateTime.Now.ToShortDateString();
            this.Ye = "200000";
        }

        /// <summary>
        /// 将字段转换为数据库插入命令
        /// </summary>
        /// <returns></returns>
        public string ToInsertString()
        {
            string result = "";
            result += "insert into zbfhz('yhzh','ye','bs','sbrq','hm') values('";
            result += this.Yhzh;
            result += "','";
            result += this.Ye;
            result += "','";
            result += this.Bs;
            result += "','";
            result += this.Sbrq;
            result += "','";
            result += this.Hm;
            result += "')";

            return result;
        }

        /// <summary>
        /// 将字段转换为数据库更新命令
        /// </summary>
        /// <returns></returns>
        public string ToUpdateString()
        {
            string result = "";
            result += "update zbfhz set ye='";
            result += this.Ye;
            result += "',bs='";
            result += this.Bs;
            result += "',sbrq='";
            result += this.Sbrq;
            result += "'";

            return result;
        }

        /// <summary>
        /// 查询某个账号共有多少笔记录
        /// </summary>
        /// <returns></returns>
        public string ToCountStringByZh()
        {
            return "select count(*) from zbfhz where yhzh='" + this.Yhzh + "'";
        }
    }
}
