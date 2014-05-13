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
namespace Business
{
    /// <summary>
    /// 贷款发放业务类
    /// </summary>
    class DK_LoanRelease : GjjBusinessSuper
    {
        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            string s = "";
            Thread.Sleep(3000);
            s = LoanReleaseMessage(recvBytes);

            LogHelper.WriteLogInfo("贷款发放", "成功");
            return Encoding.Default.GetBytes(s);
        }

        private string LoanReleaseMessage(byte[] recvBytes)
        {
            //解析请求报文
            byte[] length = new byte[4];//length:420
            byte[] transcationCode = new byte[4];//2005
            byte[] returnCode = new byte[4];
            byte[] returnInfo = new byte[60];

            byte[] batchCode = new byte[20];
            byte[] bankSeriaNum = new byte[20];//银行流水
            byte[] paryerCount = new byte[30];//付款人账号（中心）;
            byte[] payerName = new byte[60];//付款人名称（中心）;
            byte[] payBankName = new byte[60];//付款银行名称;
            byte[] recvCount = new byte[30];//收款人账号;
            byte[] recvName = new byte[60];//收款人名称;
            byte[] recvBank = new byte[60];//收款银行名称;
            byte[] money = new byte[12];//金额;

            BusinessTools.InitializeByteArray(returnInfo, 60);
            BusinessTools.InitializeByteArray(batchCode, 20);
            BusinessTools.InitializeByteArray(bankSeriaNum, 20);
            BusinessTools.InitializeByteArray(paryerCount, 30);
            BusinessTools.InitializeByteArray(payerName, 60);
            BusinessTools.InitializeByteArray(payBankName, 60);
            BusinessTools.InitializeByteArray(recvCount, 30);
            BusinessTools.InitializeByteArray(recvName, 60);
            BusinessTools.InitializeByteArray(recvBank, 60);
            BusinessTools.InitializeByteArray(money, 12);

            BusinessTools.SetByteArray(length, "0420");
            BusinessTools.SetByteArray(transcationCode, "2005");
            BusinessTools.SetByteArray(returnCode, "0000");
            BusinessTools.SetByteArray(returnInfo, "success");
            batchCode = BusinessTools.SubBytesArray(recvBytes,4,20);
            BusinessTools.SetByteArray(bankSeriaNum, "20140501");
            paryerCount = BusinessTools.SubBytesArray(recvBytes,24,30);
            payerName = BusinessTools.SubBytesArray(recvBytes, 54, 60);
            BusinessTools.SetByteArray(payBankName, "中国银行山东路支行");
            recvCount = BusinessTools.SubBytesArray(recvBytes, 114, 30);
            recvName = BusinessTools.SubBytesArray(recvBytes, 144, 60);
            recvBank = BusinessTools.SubBytesArray(recvBytes, 204, 60);
            money = BusinessTools.SubBytesArray(recvBytes, 276, 12);

            string s = "";
            s += Encoding.Default.GetString(length);
            s += Encoding.Default.GetString(transcationCode);
            s += Encoding.Default.GetString(returnCode);
            s += Encoding.Default.GetString(returnInfo);
            s += Encoding.Default.GetString(batchCode);
            s += Encoding.Default.GetString(bankSeriaNum);
            s += Encoding.Default.GetString(paryerCount);
            s += Encoding.Default.GetString(payerName);
            s += Encoding.Default.GetString(payBankName);
            s += Encoding.Default.GetString(recvCount);
            s += Encoding.Default.GetString(recvName);
            s += Encoding.Default.GetString(recvBank);
            s += Encoding.Default.GetString(money);

            return s;
        }
    }
}
