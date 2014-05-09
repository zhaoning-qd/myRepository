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
    /// 定时批量支付
    /// </summary>
    public class ZQ_TimingAndBatchPayment : GjjBusinessSuper
    {
        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            string s = "";
            Thread.Sleep(1000);
            s = ZQ_DingShiPiLiangZhiFuMessage(recvBytes);

            LogHelper.WriteLogInfo("定时批量支付", "成功");
            return Encoding.Default.GetBytes(s);
        }

        /// <summary>
        /// 定时批量支付;
        /// </summary>
        /// <param name="recvBytes"></param>
        /// <returns></returns>
        private string ZQ_DingShiPiLiangZhiFuMessage(byte[] recvBytes)
        {
            byte[] length = new byte[4];//数据包长;
            byte[] transcationCode = new byte[4];//交易码;
            byte[] returnCode = new byte[4];//返回值;
            byte[] batchCode = new byte[20];//批次号;
            byte[] fileName = new byte[30];//文件名称;
            byte[] payCount = new byte[30];
            byte[] sumCount = new byte[6];//笔数;
            byte[] money = new byte[12];//金额;
            byte[] remark = new byte[60];//备注;

            transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            batchCode = BusinessTools.SubBytesArray(recvBytes, 4, 20);
            fileName = BusinessTools.SubBytesArray(recvBytes, 24, 30);
            payCount = BusinessTools.SubBytesArray(recvBytes, 54, 30);
            sumCount = BusinessTools.SubBytesArray(recvBytes, 84, 6);
            money = BusinessTools.SubBytesArray(recvBytes, 90, 12);
            remark = BusinessTools.SubBytesArray(recvBytes, 102, 60);

            BusinessTools.SetByteArray(length, "0420");
            BusinessTools.SetByteArray(returnCode, "0000");

            string s = "";
            s += Encoding.Default.GetString(length);
            s += Encoding.Default.GetString(transcationCode);
            s += Encoding.Default.GetString(returnCode);
            s += Encoding.Default.GetString(batchCode);
            s += Encoding.Default.GetString(fileName);
            s += Encoding.Default.GetString(sumCount);
            s += Encoding.Default.GetString(money);
            s += Encoding.Default.GetString(remark);

            return s;
        }
    }
}
