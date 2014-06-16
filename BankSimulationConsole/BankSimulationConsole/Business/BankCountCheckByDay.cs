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
    /// 银行记账日终对账
    /// </summary>
    public class BankCountCheckByDay:GjjBusinessSuper
    {
        //请求报文字段
        private string jym = "";//交易码
        private string yhzh = "";//银行账号
        private string qsrq = "";//起始日期
        private string zzrq = "";//终止日期

        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            //解析报文
            byte[] transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            byte[] bankCount = BusinessTools.SubBytesArray(recvBytes, 4, 30);
            byte[] qsrq = BusinessTools.SubBytesArray(recvBytes, 34, 8);
            byte[] zzrq = BusinessTools.SubBytesArray(recvBytes, 42, 8);

            this.jym = Encoding.Default.GetString(transcationCode).TrimEnd();
            this.yhzh = Encoding.Default.GetString(bankCount).TrimEnd();
            this.qsrq = Encoding.Default.GetString(qsrq).TrimEnd();
            this.zzrq = Encoding.Default.GetString(zzrq).TrimEnd();

            //生成对账明细
            string fileName = GenetrateCountCheckingFile(this.jym,this.yhzh,this.qsrq,this.zzrq);
            //生成返回报文
            string s = "";
            s = BankCountCheckMessage(this.jym, this.yhzh, this.qsrq, this.zzrq,fileName);

            LogHelper.WriteLogInfo("银行记账日终对账", "对账成功");
            return Encoding.Default.GetBytes(s);
        }

        /// <summary>
        /// 银行日记账对账--生成对账明细
        /// </summary>
        /// <param name="transcationCode"></param>
        /// <param name="bankCount"></param>
        /// <param name="qsrq"></param>
        /// <param name="zzrq"></param>
        /// <returns></returns>
        private string GenetrateCountCheckingFile(string transcationCode, string bankCount, string qsrq, string zzrq)
        {
            string fileName = "";
            fileName += "YHDZ";
            fileName += bankCount;
            fileName += "_";
            fileName += qsrq;
            fileName += "_";
            fileName += zzrq;

            //详细文件内容

            return fileName;
        }

        /// <summary>
        /// 产生银行日记账对账返回报文
        /// </summary>
        /// <param name="transcationCode"></param>
        /// <param name="bankCount"></param>
        /// <param name="qsrq"></param>
        /// <param name="zzrq"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string BankCountCheckMessage(string transcationCode, string bankCount, string qsrq,
            string zzrq, string fileName)
        {
            byte[] length = new byte[4];//length = 250
            BusinessTools.InitializeByteArray(length, 4);
            BusinessTools.SetByteArray(length, "0250");

            byte[] bTranscationCode = new byte[4];//交易码
            BusinessTools.InitializeByteArray(bTranscationCode, 4);
            BusinessTools.SetByteArray(bTranscationCode, transcationCode);

            byte[] returnCode = new byte[4];
            BusinessTools.InitializeByteArray(returnCode, 4);
            BusinessTools.SetByteArray(returnCode, "0000");

            byte[] countCheckingName = new byte[20];//对账单名称
            BusinessTools.InitializeByteArray(countCheckingName, 20);
            BusinessTools.SetByteArray(countCheckingName, "山东路分行对账单");

            byte[] countName = new byte[60];//账户名称
            BusinessTools.InitializeByteArray(countName, 60);
            BusinessTools.SetByteArray(countName, "杭州住房公积金管理中心萧山分中心");

            byte[] bBankCount = new byte[30];//银行账号
            BusinessTools.InitializeByteArray(bBankCount, 30);
            BusinessTools.SetByteArray(bBankCount, bankCount);

            byte[] bQsrq = new byte[8];//起始日期
            BusinessTools.InitializeByteArray(bQsrq, 8);
            BusinessTools.SetByteArray(bQsrq, qsrq);

            byte[] bZzrq = new byte[8];//终止日期
            BusinessTools.InitializeByteArray(bZzrq, 8);
            BusinessTools.SetByteArray(bZzrq, zzrq);

            byte[] sumRecords = new byte[8];//汇总记录数
            BusinessTools.InitializeByteArray(sumRecords, 8);
            BusinessTools.SetByteArray(sumRecords, "5");

            byte[] sumJFRecords = new byte[8];//汇总借方笔数
            BusinessTools.InitializeByteArray(sumJFRecords, 8);
            BusinessTools.SetByteArray(sumJFRecords, "2");

            byte[] sumJFMoney = new byte[12];//汇总借方发生额
            BusinessTools.InitializeByteArray(sumJFMoney, 12);
            BusinessTools.SetByteArray(sumJFMoney, "1000000");

            byte[] sumDFRecords = new byte[8];//汇总贷方笔数
            BusinessTools.InitializeByteArray(sumDFRecords, 8);
            BusinessTools.SetByteArray(sumDFRecords, "3");

            byte[] sumDFMoney = new byte[12];//汇总贷方发生额
            BusinessTools.InitializeByteArray(sumDFMoney, 12);
            BusinessTools.SetByteArray(sumDFMoney, "1000000");

            byte[] dzrq = new byte[8];//对账日期
            BusinessTools.InitializeByteArray(dzrq, 8);
            DateTime dt = DateTime.Now;
            string strDate = dt.ToString("yyyyMMdd");
            BusinessTools.SetByteArray(dzrq, strDate);

            byte[] countCheckFileName = new byte[60];//对账文件名称
            BusinessTools.InitializeByteArray(countCheckFileName, 60);
            BusinessTools.SetByteArray(countCheckFileName, fileName);

            string result = "";
            result += Encoding.Default.GetString(length);
            result += Encoding.Default.GetString(bTranscationCode);
            result += Encoding.Default.GetString(returnCode);
            result += Encoding.Default.GetString(countCheckingName);
            result += Encoding.Default.GetString(countName);
            result += Encoding.Default.GetString(bBankCount);
            result += Encoding.Default.GetString(bQsrq);
            result += Encoding.Default.GetString(bZzrq);
            result += Encoding.Default.GetString(sumRecords);
            result += Encoding.Default.GetString(sumJFRecords);
            result += Encoding.Default.GetString(sumJFMoney);
            result += Encoding.Default.GetString(sumDFRecords);
            result += Encoding.Default.GetString(sumDFMoney);
            result += Encoding.Default.GetString(dzrq);
            result += Encoding.Default.GetString(countCheckFileName);

            return result;
        }
    }
}
