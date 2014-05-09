using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTools;

namespace IBusiness
{
    /// <summary>
    /// 所有公积金业务的基类
    /// </summary>
    public abstract class GjjBusinessSuper
    {
        /// <summary>
        /// 执行业务操作
        /// </summary>
        public abstract byte[] HandleBusiness(byte[] recvBytes, string whichBank);

        ///// <summary>
        ///// 产生返回报文
        ///// </summary>
        ///// <param name="recvBytes"></param>
        ///// <returns></returns>
        //public abstract string GenerateMessage(byte[] recvBytes);

        /// <summary>
        /// 将操作异常写入日志
        /// </summary>
        public void WriteLogException(string businessName,Exception ex)
        {
            LogHelper.WriteLogException(businessName, ex);
        }

        /// <summary>
        /// 将错误信息写入日志
        /// </summary>
        /// <param name="businessName"></param>
        /// <param name="errorMessage"></param>
        public void WriteLogError(string businessName, string errorMessage)
        {
            LogHelper.WriteLogError(businessName, errorMessage);
        }

        /// <summary>
        /// 将操作结果写入日志
        /// </summary>
        /// <param name="businessName"></param>
        /// <param name="info"></param>
        public void WriteLogInfo(string businessName, string info)
        {
            LogHelper.WriteLogInfo(businessName, info);
        }

        /// <summary>
        /// 在控制台显示信息
        /// </summary>
        public void DisplayConsoleInfo(string businessName, string msg)
        {
        }

        ///////////***************************处理两特殊业务******************
        //1.小额支付代扣--当收到GJJ的请求报文时，马上返回响应报文
        /// <summary>
        /// 返回给GJJ的信息--当服务端一接收到GJJ发送的报文时，就返回该应答报文;
        /// </summary>
        /// <param name="recvBytes"></param>
        /// <returns></returns>
        public byte[] XiaoEZhiFuDaiDouOnLineMessage(byte[] recvBytes)
        {
            string s = "0028";
            string transcationCode, batchCode, returnCode;
            transcationCode = Encoding.UTF8.GetString(BusinessTools.SubBytesArray(recvBytes, 0, 4));
            batchCode = Encoding.UTF8.GetString(BusinessTools.SubBytesArray(recvBytes, 4, 20));
            returnCode = "0000";

            s += transcationCode;
            s += returnCode;
            s += batchCode;

            return Encoding.UTF8.GetBytes(s);

        }

        //2.贷款批量收回发起
        /// <summary>
        /// 贷款批量收回发起--返回给GJJ的信息;
        /// </summary>
        /// <param name="recvBytes"></param>
        /// <returns></returns>
        public byte[] LoanBatchWithDrawOnLineMessage(byte[] recvBytes)
        {
            string s = "";
            byte[] transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            byte[] batchCode = BusinessTools.SubBytesArray(recvBytes, 4, 20);
            byte[] sumRecords = BusinessTools.SubBytesArray(recvBytes, 24, 6);
            byte[] sumMoney = BusinessTools.SubBytesArray(recvBytes, 30, 12);
            byte[] fileName = BusinessTools.SubBytesArray(recvBytes, 42, 30);
            byte[] returnCode = Encoding.UTF8.GetBytes("0000");

            s += "0028";
            s += Encoding.UTF8.GetString(transcationCode);
            s += Encoding.UTF8.GetString(returnCode);
            s += Encoding.UTF8.GetString(batchCode);

            return Encoding.UTF8.GetBytes(s);
        }
    }
}
