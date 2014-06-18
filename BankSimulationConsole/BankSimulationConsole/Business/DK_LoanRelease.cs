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
using Entities.BllModels;
using Entities;
namespace Business
{
    /// <summary>
    /// 贷款发放业务类
    /// </summary>
    class DK_LoanRelease : GjjBusinessSuper
    {
        /// <summary>
        /// 业务实体
        /// </summary>
        DkfyModel dkfy = new DkfyModel();
        private string yhls = string.Empty;//银行流水

        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            string s = "";
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

            dkfy.Jym = "2005";
            dkfy.Pch = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 4, 20)).TrimEnd();
            Random random = new Random();
            this.yhls =  BusinessTools.GenerateLongBankSerialNum(random.Next(99));
            dkfy.Skrzh = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 114, 30)).TrimEnd();
            dkfy.Fkrzh = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 24, 30)).TrimEnd();
            dkfy.Fkrmc = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 54, 60)).TrimEnd();
            dkfy.Je = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 276, 12)).TrimEnd();
            dkfy.Skyhmc = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 144, 60)).TrimEnd();

            BusinessTools.SetByteArray(length, "0420");
            BusinessTools.SetByteArray(transcationCode, dkfy.Jym);
            BusinessTools.SetByteArray(returnCode, "0000");
            BusinessTools.SetByteArray(returnInfo, "success");
            batchCode = BusinessTools.SubBytesArray(recvBytes,4,20);
            BusinessTools.SetByteArray(bankSeriaNum, this.yhls);
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

        /// <summary>
        /// 更新账表分户账和账表明细账
        /// </summary>
        private void UpdateZbInfo()
        {
            ZbfhzEntity zbfhz = new ZbfhzEntity();
            ZbmxzEntity zbmxz = new ZbmxzEntity();

            zbmxz.Zh = dkfy.Skrzh;
            int iBs = BusinessHelper.GetCountByZh(zbmxz);
            zbmxz.Bc = (iBs + 1).ToString();
            zbmxz.Fse = dkfy.Je;
            zbmxz.Yhls = this.yhls;
            zbmxz.Pjhm = dkfy.Pch;
            zbmxz.Jdbz = "2";
            zbmxz.Ywlx = "1";
            zbmxz.Dfzh = dkfy.Fkrzh;
            zbmxz.Dfhm = dkfy.Fkrmc;
            zbmxz.Zxjsh = dkfy.Fkrzh;

            zbfhz.Yhzh = zbmxz.Zh;
            zbfhz.Bs = zbmxz.Bc;
            zbfhz.Hm = dkfy.Skyhmc;

            BusinessHelper.UpateZbfhzAndZbmxz(zbmxz, zbfhz);

        }
    }
}
