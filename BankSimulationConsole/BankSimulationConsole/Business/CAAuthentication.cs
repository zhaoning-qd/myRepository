using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools;
using IBusiness;
using System.IO;
using System.Threading;
using System.Configuration;

namespace Business
{
    /// <summary>
    /// CA认证
    /// </summary>
    public class CAAuthentication : BusinessLaunchedByBankSuper
    {
        /// <summary>
        /// 业务处理
        /// </summary>
        /// <returns></returns>
        public override byte[] HandleBusiness()
        {
            string s = "";
            s = CAAuthenticationMessage();

            LogHelper.WriteLogInfo("CA认证", "发送验证请求成功");
            return Encoding.Default.GetBytes(s);
        }       

        /// <summary>
        /// CA认证
        /// </summary>
        /// <param name="recvBytes"></param>
        /// <returns></returns>
        public string CAAuthenticationMessage()
        {
            byte[] length = new byte[4];
            BusinessTools.InitializeByteArray(length, 4);
            BusinessTools.SetByteArray(length, "0096");

            byte[] transcationCode = new byte[4];
            BusinessTools.InitializeByteArray(length, 4);
            BusinessTools.SetByteArray(transcationCode, "w001");

            byte[] dwzh = new byte[12];
            BusinessTools.InitializeByteArray(dwzh, 12);
            BusinessTools.SetByteArray(dwzh, "110958674325");

            byte[] requestNO = new byte[21];
            BusinessTools.InitializeByteArray(requestNO, 21);
            BusinessTools.SetByteArray(requestNO, "345683726453647564732");

            byte[] isCASuccess = new byte[1];
            BusinessTools.SetByteArray(isCASuccess, "2");

            byte[] beiz = new byte[60];
            BusinessTools.InitializeByteArray(beiz, 60);
            BusinessTools.SetByteArray(beiz, "nothing");

            string s = "";
            s += Encoding.Default.GetString(length);
            s += Encoding.Default.GetString(transcationCode);
            s += Encoding.Default.GetString(dwzh);
            s += Encoding.Default.GetString(requestNO);
            s += Encoding.Default.GetString(isCASuccess);
            s += Encoding.Default.GetString(beiz);

            return s;
        }
    }
}
