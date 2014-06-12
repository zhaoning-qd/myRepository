using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    /// <summary>
    /// 账表明细账实体
    /// </summary>
    public class ZbmxzEntity
    {
        /// <summary>
        /// 笔次
        /// </summary>
        public string Bc { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Zh { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public string Jyrq { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        public string Jysj { get; set; }

        /// <summary>
        /// 发生额
        /// </summary>
        public string Fse { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public string Ye { get; set; }

        /// <summary>
        /// 银行流水
        /// </summary>
        public string Yhls { get; set; }

        /// <summary>
        /// 票据号码
        /// </summary>
        public string Pjhm { get; set; }

        /// <summary>
        /// 借款标志
        /// </summary>
        public string Jdbz { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string Ywlx { get; set; }

        /// <summary>
        /// 对方账号
        /// </summary>
        public string Dfzh { get; set; }

        /// <summary>
        /// 对方户名
        /// </summary>
        public string Dfhm { get; set; }

        /// <summary>
        /// 中心结算账号
        /// </summary>
        public string Zxjsh { get; set; }

        /// <summary>
        /// 构造函数--初始化日期和时间
        /// </summary>
        public ZbmxzEntity()
        {
            this.Jyrq = DateTime.Now.ToShortDateString();
            this.Jysj = DateTime.Now.ToLongTimeString();
        }

        /// <summary>
        /// 将字段转换为数据库插入命令
        /// </summary>
        /// <returns></returns>
        public string ToInsertString()
        {
            string s = "";
            s += "insert into zbmxz('bc','zh','jyrq','jysj','fse','ye','yhls','pjhm','jdbz','ywlx','dfzh','dfhm','zxjsh') values('";
            s += this.Bc;
            s += "','";
            s += this.Zh;
            s += "','";
            s += this.Jyrq;
            s += "','";
            s += this.Jysj;
            s += "','";
            s += this.Fse;
            s += "','";
            s += this.Ye;
            s += "','";
            s += this.Yhls;
            s += "','";
            s += this.Pjhm;
            s += "','";
            s += this.Jdbz;
            s += "','";
            s += this.Ywlx;
            s += "','";
            s += this.Dfzh;
            s += "','";
            s += this.Dfhm;
            s += "','";
            s += this.Zxjsh;
            s += "')";

            return s;
        }

        /// <summary>
        /// 查询某个账号共有多少笔记录
        /// </summary>
        /// <returns></returns>
        public string ToCountStringByZh()
        {
            return "select count(*) from zbmxz where zh='" + this.Zh + "'";
        }

    }
}
