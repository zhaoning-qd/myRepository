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
    /// 贷款对账信息分发
    /// </summary>
    public class LoanCheckingInfoDistribution:GjjBusinessSuper
    {
        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            string s = "";
            Thread.Sleep(3000);
            s = LoanCheckInfoDistributeMessage(recvBytes);

            LogHelper.WriteLogInfo("贷款对账信息分发", "成功");
            return Encoding.Default.GetBytes(s);
        }

        /// <summary>
        /// 贷款对账信息分发
        /// </summary>
        /// <param name="recvBytes"></param>
        /// <returns></returns>
        public string LoanCheckInfoDistributeMessage(byte[] recvBytes)
        {
            byte[] transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            byte[] fileName = BusinessTools.SubBytesArray(recvBytes, 4, 60);

            string s = "0068";
            s += Encoding.Default.GetString(transcationCode);
            s += "0000";
            s += Encoding.Default.GetString(fileName);

            return s;
        }
    }
}
