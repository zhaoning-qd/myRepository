using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools;
using IBusiness;
using System.IO;
using System.Threading;
using IDataAccess;
using System.Configuration;
using Entities;

namespace Business
{
    /// <summary>
    /// 贷款单笔扣款
    /// </summary>
    public class DK_LoanSingleWithHold:GjjBusinessSuper
    {

        private string yhkh;//银行卡号
        private decimal ykje;//应扣金额
        private decimal skje;//实扣金额
        private string hth;//合同号
        private string xm;//姓名
        private string sfz;//身份证号
        private string kkhhh;//卡开户行号
        private string skrzh;//收款人账号
        private string skrmx;//收款人名称
        private string skyhmx;//收款银行名称
        private int ye;//余额

        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            string s = "";
            Thread.Sleep(3000);
            s = LoanSingleWithHoldMessage(recvBytes);

            //写zbfhz和zbmxz
            UpateZbfhzAndZbmxz(recvBytes);

            LogHelper.WriteLogInfo("贷款单笔扣款", "成功");
            return Encoding.Default.GetBytes(s);
        }

        private string LoanSingleWithHoldMessage(byte[] recvBytes)
        {
            byte[] length = new byte[4];//length:274
            byte[] transcationCode = new byte[4];//2008
            byte[] returnCode = new byte[4];
            byte[] returnInfo = new byte[60];
            byte[] type = new byte[2];
            byte[] cardNO = new byte[30];
            byte[] moneyToMight = new byte[10];//应扣金额
            byte[] moneyToBe = new byte[10];//实扣金额
            byte[] xm = new byte[20];
            byte[] sfz = new byte[18];
            byte[] cebz = new byte[1];//差额标志
            byte[] hkqc = new byte[3];//还款其次
            byte[] hth = new byte[12];//合同号
            byte[] ykbj = new byte[10];//应扣本金
            byte[] yklx = new byte[10];//应扣利息
            byte[] bankSeriaNum = new byte[20];//银行流水
            byte[] obligation = new byte[60];//预留

            //初始化
            BusinessTools.InitializeByteArray(returnCode, 4);
            BusinessTools.InitializeByteArray(returnInfo, 60);
            BusinessTools.InitializeByteArray(bankSeriaNum, 20);

            //赋值
            transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            BusinessTools.SetByteArray(returnCode, "0000");
            BusinessTools.SetByteArray(returnInfo, "success");
            type = BusinessTools.SubBytesArray(recvBytes, 4, 2);
            cardNO = BusinessTools.SubBytesArray(recvBytes, 6, 30);
            moneyToMight = BusinessTools.SubBytesArray(recvBytes, 36, 10);
            moneyToBe = BusinessTools.SubBytesArray(recvBytes, 46, 10);
            xm = BusinessTools.SubBytesArray(recvBytes, 56, 20);
            sfz = BusinessTools.SubBytesArray(recvBytes, 76, 18);
            cebz = BusinessTools.SubBytesArray(recvBytes, 94, 1);
            hkqc = BusinessTools.SubBytesArray(recvBytes, 95, 3);
            hth = BusinessTools.SubBytesArray(recvBytes, 98, 12);
            ykbj = BusinessTools.SubBytesArray(recvBytes, 110, 10);
            yklx = BusinessTools.SubBytesArray(recvBytes, 120, 10);
            BusinessTools.SetByteArray(bankSeriaNum, "1234567890");
            obligation = BusinessTools.SubBytesArray(recvBytes, 150, 60);

            string s = "0274";
            s += Encoding.Default.GetString(transcationCode);
            s += Encoding.Default.GetString(returnCode);
            s += Encoding.Default.GetString(returnInfo);
            s += "22";//类型：正常扣款
            s += Encoding.Default.GetString(cardNO);
            s += Encoding.Default.GetString(moneyToMight);
            s += Encoding.Default.GetString(moneyToBe);
            s += Encoding.Default.GetString(xm);
            s += Encoding.Default.GetString(sfz);
            s += Encoding.Default.GetString(cebz);
            s += Encoding.Default.GetString(hkqc);
            s += Encoding.Default.GetString(hth);
            s += Encoding.Default.GetString(ykbj);
            s += Encoding.Default.GetString(yklx);
            s += Encoding.Default.GetString(bankSeriaNum);
            s += Encoding.Default.GetString(obligation);

            return s;

        }

        /// <summary>
        /// 更新zbfhz和zbmxz表
        /// </summary>
        private bool UpateZbfhzAndZbmxz(byte[] recvBytes)
        {
            bool zt1,zt2;
            this.ye = 200000;
            this.yhkh = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 6, 30)).TrimEnd();
            this.ykje = Convert.ToDecimal(Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 36, 10)).TrimEnd());
            this.skje = Convert.ToDecimal(Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 46, 10)).TrimEnd());
            this.xm = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 56, 20)).TrimEnd();
            this.sfz = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 76, 18)).TrimEnd();
            this.hth = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 98, 12)).TrimEnd();
            this.kkhhh = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 130, 12)).TrimEnd();
            this.skrzh = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 142, 30)).TrimEnd();
            this.skrmx = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 172, 60)).TrimEnd();
            this.skyhmx = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 232, 60)).TrimEnd();

            //账表明细账
            ZbmxzEntity zbmxz = new ZbmxzEntity();
            zbmxz.Zh = this.yhkh;
            int iBs = this.GetCountByZh(zbmxz);
            if (iBs == -1)
            {
                Console.WriteLine("查询账号{0}对应的笔数出错", zbmxz.Zh);
                return false;
            }
            else
            {
                zbmxz.Bc = (iBs + 1).ToString();
                zbmxz.Jyrq = DateTime.Now.ToShortDateString();
                zbmxz.Jysj = DateTime.Now.ToLongTimeString();
                zbmxz.Fse = this.skje.ToString();
                zbmxz.Ye = this.ye.ToString();

                Random radom = new Random();
                zbmxz.Yhls = BusinessTools.GenerateLongBankSerialNum(radom.Next(99));
                zbmxz.Pjhm = this.hth;
                zbmxz.Jdbz = "2";
                zbmxz.Ywlx = "1";
                zbmxz.Dfzh = this.skrzh;
                zbmxz.Dfhm = this.skrmx;
                zbmxz.Zxjsh = this.skyhmx;

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
            ZbfhzEntity zbfhz = new ZbfhzEntity();
            zbfhz.Yhzh = this.yhkh;
            zbfhz.Ye = this.ye.ToString();
            zbfhz.Bs = zbmxz.Bc;
            zbfhz.Sbrq = DateTime.Now.ToShortDateString();
            zbfhz.Hm = this.xm;

            int num = this.GetCountByZh(zbfhz);

            if (num == -1)
            {
                Console.WriteLine("查询账号{0}是否存在时出错", zbfhz.Yhzh);
                return false;
            }
            if (num == 0)
            {
                if (this.ExecuteUpdateCmd(zbfhz.ToInsertString()))
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
                if (this.ExecuteUpdateCmd(zbfhz.ToUpdateString()))
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

        /// <summary>
        /// 查询zbmxz中某账号的笔数
        /// </summary>
        /// <returns></returns>
        private int GetCountByZh(ZbmxzEntity zbmxz)
        {
            string assemblyName = "DataAccess";
            string namespaceName = "DataAccess";
            string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
            IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

            return iDB2Operation.ExecuteCountQuery(zbmxz.ToCountStringByZh());
        }

        /// <summary>
        /// 查询zbmxz中某账号的笔数
        /// </summary>
        /// <returns></returns>
        private int GetCountByZh(ZbfhzEntity zbfhz)
        {
            string assemblyName = "DataAccess";
            string namespaceName = "DataAccess";
            string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
            IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

            return iDB2Operation.ExecuteCountQuery(zbfhz.ToCountStringByZh());
        }

        /// <summary>
        /// 执行更新或插入命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private bool ExecuteUpdateCmd(string cmd)
        {
            string assemblyName = "DataAccess";
            string namespaceName = "DataAccess";
            string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
            IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

            return iDB2Operation.ExecuteDB2Update(cmd);

        }

    }
}
