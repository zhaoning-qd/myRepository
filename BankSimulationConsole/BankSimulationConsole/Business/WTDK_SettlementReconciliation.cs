using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBusiness;
using CommonTools;
using System.Threading;
using System.IO;

namespace Business
{
    /// <summary>
    /// 网厅贷款--结算交易日终对账
    /// </summary>
    public class WTDK_SettlementReconciliation:GjjBusinessSuper
    {
        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            //解析请求报文
            byte[] transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            byte[] batchCodeStart = BusinessTools.SubBytesArray(recvBytes, 4, 20);
            byte[] batchCodeEnd = BusinessTools.SubBytesArray(recvBytes, 24, 20);
            byte[] jgm = BusinessTools.SubBytesArray(recvBytes, 44, 2);
            byte[] sumRecords = BusinessTools.SubBytesArray(recvBytes, 46, 6);
            byte[] sumMoney = BusinessTools.SubBytesArray(recvBytes, 52, 16);

            string s1 = Encoding.Default.GetString(transcationCode);
            string s2 = Encoding.Default.GetString(batchCodeStart);
            string s3 = Encoding.Default.GetString(batchCodeEnd);
            string s4 = Encoding.Default.GetString(jgm);
            string s5 = Encoding.Default.GetString(sumRecords);
            string s6 = Encoding.Default.GetString(sumMoney);

            string result = "";
            string fileName = "";
            Thread.Sleep(2000);
            WT_DaikuanJiesuanBusiness(whichBank, s2, s3, s4, s5, s6, out fileName);
            result = WT_DaikuanJiesuanDuizhangMessage(s1, s2, s3, s4, s5, s6, fileName);

            LogHelper.WriteLogInfo("网厅贷款--结算交易日终对账", "成功");
            return Encoding.Default.GetBytes(result);
        }

        private void WT_DaikuanJiesuanBusiness(string whichBank, string batchCodeStart, string batchCodeEnd,
            string jgm, string sumRecords, string sumMoney, out string outFileName)
        {
            string fileName = "";
            fileName += jgm;
            fileName += "D";//支取
            fileName += "_W";

            DateTime dt = new DateTime();
            string strDate = dt.ToString("yyyyMMdd");

            fileName += strDate;
            fileName += ".";
            fileName += "380910";//6位银行代号

            outFileName = fileName;

            string filePath = BusinessTools.GetFilePath(whichBank) + fileName;//文件的完整路径
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("gb2312")))
            {
                string summaryLine = string.Empty;
                summaryLine += jgm;
                summaryLine += ",";
                summaryLine += strDate;
                summaryLine += ",";
                summaryLine += sumMoney;
                summaryLine += ",";
                summaryLine += sumRecords;
                summaryLine += ",";
                sw.WriteLine(summaryLine);//汇总行
            }

            //明细行
            for (int i = 1; i <= Convert.ToInt32(sumRecords); i++)
            {
                string strTime = string.Empty;
                string detailLine = string.Empty;

                strTime = DateTime.Now.ToLongTimeString();
                detailLine += i.ToString();
                detailLine += ",";
                detailLine += strDate;
                detailLine += ",";
                detailLine += strTime;
                detailLine += ",";
                detailLine += BusinessTools.GenerateBatchCode("110000000", i);//批次号
                detailLine += ",";
                detailLine += BusinessTools.GenerateName("李", i);
                detailLine += ",";
                detailLine += BusinessTools.GenerateBankCount("62220238040567399", i);
                detailLine += ",";
                detailLine += "1000.00";
                detailLine += ",";
                detailLine += BusinessTools.GenerateBankSerialNum(i);//银行流水
                detailLine += ",";
                detailLine += "1";//记账标志
                detailLine += ",";
                detailLine += BusinessTools.GenerateBankSerialNum(i);//备注中添写银行流水号
                detailLine += ",";

                using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("gb2312")))
                {
                    sw.WriteLine(detailLine);
                }
            }
        }

        /// <summary>
        /// 贷款结算交易日终对账响应报文
        /// </summary>
        /// <param name="transcationCode"></param>
        /// <param name="batchCodeStart"></param>
        /// <param name="batchCodeEnd"></param>
        /// <param name="jgm"></param>
        /// <param name="sumRecords"></param>
        /// <param name="sumMoney"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string WT_DaikuanJiesuanDuizhangMessage(string transcationCode,
                                                                     string batchCodeStart,
                                                                     string batchCodeEnd,
                                                                     string jgm,
                                                                     string sumRecords,
                                                                     string sumMoney,
                                                                     string fileName)
        {
            string s = "";
            byte[] length = new byte[4];
            byte[] bTranCode = new byte[4];
            byte[] bRetuCode = new byte[4];
            byte[] bRetuValueInfo = new byte[60];
            byte[] bJgm = new byte[2];
            byte[] bFileName = new byte[30];
            byte[] bSumRecords = new byte[6];
            byte[] bSumMoney = new byte[16];

            BusinessTools.SetByteArray(length, "0122");
            BusinessTools.SetByteArray(bTranCode, "3009");
            BusinessTools.SetByteArray(bRetuCode, "0000");
            BusinessTools.SetByteArray(bJgm, jgm);

            BusinessTools.InitializeByteArray(bRetuValueInfo, 60);

            BusinessTools.InitializeByteArray(bFileName, 60);
            BusinessTools.SetByteArray(bFileName, fileName);
            BusinessTools.InitializeByteArray(bSumRecords, 6);
            BusinessTools.SetByteArray(bSumRecords, sumRecords);
            BusinessTools.InitializeByteArray(bSumMoney, 16);
            BusinessTools.SetByteArray(bSumMoney, sumMoney);

            s += Encoding.Default.GetString(length);
            s += Encoding.Default.GetString(bTranCode);
            s += Encoding.Default.GetString(bRetuCode);
            s += Encoding.Default.GetString(bRetuValueInfo);
            s += Encoding.Default.GetString(bJgm);
            s += Encoding.Default.GetString(bFileName);
            s += Encoding.Default.GetString(bSumRecords);
            s += Encoding.Default.GetString(bSumMoney);

            return s;
        }
    }
}
