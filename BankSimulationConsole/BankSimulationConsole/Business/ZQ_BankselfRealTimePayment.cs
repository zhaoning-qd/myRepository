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
    /// 支取支付--本行实时
    /// </summary>
    public class ZQ_BankselfRealTimePayment : GjjBusinessSuper
    {
        ZbfhzEntity zbfhz = new ZbfhzEntity();
        ZbmxzEntity zbmxz = new ZbmxzEntity();

        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            string s = "";
            Thread.Sleep(1000);
            s = ZQ_ZhiQuZhiFuBenHangMessage(recvBytes);

            LogHelper.WriteLogInfo("支取支付--本行实时", "成功");
            return Encoding.Default.GetBytes(s);
        }

        private string ZQ_ZhiQuZhiFuBenHangMessage(byte[] recvBytes)
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

            //赋值byte[];
            BusinessTools.SetByteArray(length, "0420");
            BusinessTools.SetByteArray(transcationCode, "2003");
            BusinessTools.SetByteArray(returnCode, "0000");
            BusinessTools.SetByteArray(batchCode, "1100090673");
            BusinessTools.SetByteArray(bankSerialNum, "2003033101");
            BusinessTools.SetByteArray(paryerCount, "4857685747385947584");
            BusinessTools.SetByteArray(payerName, "青岛市住房公积金管理中心");
            BusinessTools.SetByteArray(payBankName, "中国工商银行青岛市市南区山东路分行");
            BusinessTools.SetByteArray(recvCount, "4857685747385947123");
            BusinessTools.SetByteArray(recvName, "中国中铁青岛建设局市南区山东路分局");
            BusinessTools.SetByteArray(recvBank, "中国建设银行青岛市市南区山东路分行");
            BusinessTools.SetByteArray(money, "3434354");
            BusinessTools.SetByteArray(remark, "This is the test message from GJJ");

            string s = "";
            s += Encoding.Default.GetString(length);
            s += Encoding.Default.GetString(transcationCode);
            s += Encoding.Default.GetString(returnCode);
            s += Encoding.Default.GetString(batchCode);
            s += Encoding.Default.GetString(bankSerialNum);
            s += Encoding.Default.GetString(paryerCount);
            s += Encoding.Default.GetString(payerName);
            s += Encoding.Default.GetString(payBankName);
            s += Encoding.Default.GetString(recvCount);
            s += Encoding.Default.GetString(recvName);
            s += Encoding.Default.GetString(recvBank);
            s += Encoding.Default.GetString(money);
            s += Encoding.Default.GetString(remark);

            return s;
        }
    }
}
