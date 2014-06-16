using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBusiness;
using CommonTools;
using System.Threading;
using Entities;

namespace Business
{
    /// <summary>
    /// 支取支付--跨行实时
    /// </summary>
    public class ZQ_Inter_BankRealTimePayment:GjjBusinessSuper
    {
        //请求报文字段
        private string jym = "";//交易码
        private string pch = "";//批次号
        private string fkrzh = "";//付款人账号(中心)
        private string fkrmc = "";//付款人名称(中心)
        private string fkyhmc = "";//付款银行名称
        private string skrzh = "";//收款人账号
        private string skrmc = "";//收款人名称
        private string skyhmc = "";//收款银行名称
        private string skrkhhh = "";//收款人开户行号
        private string je = "";//金额
        private string beiz = "";//备注

        private string yhls = "";//银行流水


        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            string s = "";
            Thread.Sleep(1000);
            s = ZQ_ZhiQuZhiFuKuaHangMessage(recvBytes);

            LogHelper.WriteLogInfo("支取支付--跨行实时", "成功");
            return Encoding.Default.GetBytes(s);
        }

        /// <summary>
        /// 支取支付--跨行;
        /// </summary>
        /// <param name="message"></param>
        /// <param name="zt"></param>
        /// <returns></returns>
        private string ZQ_ZhiQuZhiFuKuaHangMessage(byte[] recvBytes)
        {
            byte[] length = new byte[4];//数据包长;
            byte[] transcationCode = new byte[4];//交易码;
            byte[] returnCode = new byte[4];//返回值;
            byte[] batchCode = new byte[20];//批次号;
            byte[] bankSerialNum = new byte[20];//银行流水号;
            byte[] paryerCount = new byte[30];//付款人账号（中心）;
            byte[] payerName = new byte[60];//付款人名称（中心）;
            byte[] payBankName = new byte[60];//付款银行名称;
            byte[] recvCount = new byte[30];//收款人账号;
            byte[] recvName = new byte[60];//收款人名称;
            byte[] recvBank = new byte[60];//收款银行名称;
            byte[] money = new byte[12];//金额;
            byte[] remark = new byte[60];//备注;



            //初始化byte[];

            BusinessTools.InitializeByteArray(batchCode, 20);
            BusinessTools.InitializeByteArray(bankSerialNum, 20);
            BusinessTools.InitializeByteArray(paryerCount, 30);
            BusinessTools.InitializeByteArray(payerName, 60);
            BusinessTools.InitializeByteArray(payBankName, 60);
            BusinessTools.InitializeByteArray(recvCount, 30);
            BusinessTools.InitializeByteArray(recvName, 60);
            BusinessTools.InitializeByteArray(recvBank, 60);
            BusinessTools.InitializeByteArray(money, 12);
            BusinessTools.InitializeByteArray(remark, 60);

            //解析请求报文
            this.jym = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 0, 4)).TrimEnd();
            this.pch = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 4, 20)).TrimEnd();
            this.fkrzh = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 24, 30)).TrimEnd();
            this.fkrmc = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 54, 60)).TrimEnd();
            this.fkyhmc = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 114, 60)).TrimEnd();
            this.skrzh = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 174, 30)).TrimEnd();
            this.skrmc = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 204, 60)).TrimEnd();
            this.skyhmc = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 264, 60)).TrimEnd();
            this.skrkhhh = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 324, 12)).TrimEnd();
            this.je = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 336, 12)).TrimEnd();
            this.beiz = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 348, 60)).TrimEnd();

            //赋值byte[];
            BusinessTools.SetByteArray(length, "0420");
            BusinessTools.SetByteArray(transcationCode, this.jym);
            BusinessTools.SetByteArray(returnCode, "0000");
            BusinessTools.SetByteArray(batchCode, this.pch);
            //银行流水
            Random radom = new Random();
            this.yhls = BusinessTools.GenerateLongBankSerialNum(radom.Next(99));
            BusinessTools.SetByteArray(bankSerialNum,this.yhls );
            
            BusinessTools.SetByteArray(paryerCount, this.fkrzh);
            BusinessTools.SetByteArray(payerName, this.fkrmc);
            BusinessTools.SetByteArray(payBankName, this.fkyhmc);
            BusinessTools.SetByteArray(recvCount, this.skrzh);
            BusinessTools.SetByteArray(recvName, this.skrmc);
            BusinessTools.SetByteArray(recvBank, this.skyhmc);
            BusinessTools.SetByteArray(money, this.je);
            BusinessTools.SetByteArray(remark, this.beiz);

            string s = "";
            s += Encoding.Default.GetString(length);
            s += Encoding.Default.GetString(transcationCode);
            s += Encoding.Default.GetString(returnCode);
            s += Encoding.Default.GetString(batchCode);
            s += Encoding.Default.GetString(bankSerialNum);
            s += Encoding.Default.GetString(paryerCount);
            s += Encoding.Default.GetString(payerName);
            s += Encoding.Default.GetString(payBankName);
            s += Encoding.Default.GetString(recvName);
            s += Encoding.Default.GetString(recvCount);
            s += Encoding.Default.GetString(recvBank);
            s += Encoding.Default.GetString(money);
            s += Encoding.Default.GetString(remark);

            return s;
        }

        /// <summary>
        /// 更新账表分户账和账表明细账
        /// </summary>
        private void UpdateZbInfo()
        {
            ZbfhzEntity zbfhz = new ZbfhzEntity();
            ZbmxzEntity zbmxz = new ZbmxzEntity();

            zbmxz.Zh = this.skrzh;
            int iBs = BusinessHelper.GetCountByZh(zbmxz);
            zbmxz.Bc = (iBs + 1).ToString();
            zbmxz.Fse = this.je;
            zbmxz.Yhls = this.yhls;
            zbmxz.Pjhm = this.pch;
            zbmxz.Jdbz = "2";
            zbmxz.Ywlx = "1";
            zbmxz.Dfzh = this.fkrzh;
            zbmxz.Dfhm = this.fkrmc;
            zbmxz.Zxjsh = this.fkrzh;

            zbfhz.Yhzh = zbmxz.Zh;
            zbfhz.Bs = zbmxz.Bc;
            zbfhz.Hm = this.skrmc;

            BusinessHelper.UpateZbfhzAndZbmxz(zbmxz, zbfhz);

        }
    }
}
