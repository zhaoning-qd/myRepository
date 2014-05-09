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
    /// 贷款单笔扣款
    /// </summary>
    public class DK_LoanSingleWithHold:GjjBusinessSuper
    {
        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            string s = "";
            Thread.Sleep(3000);
            s = LoanSingleWithHoldMessage(recvBytes);

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

    }
}
