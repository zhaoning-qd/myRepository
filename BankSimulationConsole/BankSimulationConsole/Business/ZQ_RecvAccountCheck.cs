using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBusiness;
using CommonTools;
using System.Threading;

namespace Business
{
    /// <summary>
    /// 支取业务--收账户检验
    /// </summary>
    public class ZQ_RecvAccountCheck:GjjBusinessSuper
    {
        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            //具体处理过程;
            Thread.Sleep(1000);

            LogHelper.WriteLogInfo("支取业务--收账户检验", "成功");
            return ZQ_ShouZhangHuJianYanMessage(recvBytes);
        }

        /// <summary>
        /// 生成返回报文
        /// </summary>
        /// <param name="recvBytes"></param>
        /// <returns></returns>
        private  byte[] ZQ_ShouZhangHuJianYanMessage(byte[] recvBytes)
        {
            string s = "0098";
            string transcationCode, returnCode, zhmc, kh;
            transcationCode = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 0, 4));
            returnCode = "0000";
            zhmc = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 4, 60));
            kh = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 64, 30));

            s += transcationCode;
            s += returnCode;
            s += zhmc;
            s += kh;

            return Encoding.Default.GetBytes(s);

        }
    }
}
