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
    /// 卡余额查询
    /// </summary>
    public class CardBalanceQuery:GjjBusinessSuper
    {
        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            string s = "";
            Thread.Sleep(3000);
            s = KaYuEChaXunMessage(recvBytes);

            LogHelper.WriteLogInfo("卡余额查询", "查询成功");
            return Encoding.Default.GetBytes(s);
        }

        public string KaYuEChaXunMessage(byte[] recvBytes)
        {

            byte[] transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            byte[] xm = BusinessTools.SubBytesArray(recvBytes, 4, 20);
            byte[] kzh = BusinessTools.SubBytesArray(recvBytes, 24, 30);
            byte[] kkje = BusinessTools.SubBytesArray(recvBytes,54,12);//扣款金额

            byte[] returnCode = new byte[4];
            BusinessTools.InitializeByteArray(returnCode, 4);
            BusinessTools.SetByteArray(returnCode, "0000");

            byte[] kye = new byte[13];
            BusinessTools.InitializeByteArray(kye, 13);
            Double dKye = Convert.ToDouble(Encoding.Default.GetString(kkje));
            BusinessTools.SetByteArray(kye, (dKye*2).ToString());

            byte[] yezt = new byte[1];
            BusinessTools.SetByteArray(yezt, "1");

            byte[] kzt = new byte[1];
            BusinessTools.SetByteArray(kzt, "2");

            string s = "0060";
            s += Encoding.Default.GetString(transcationCode);
            s += Encoding.Default.GetString(returnCode);
            s += Encoding.Default.GetString(xm);
            s += Encoding.Default.GetString(kzh);
            s += Encoding.Default.GetString(yezt);
            s += Encoding.Default.GetString(kzt);
            return s;
        }
    }
}
