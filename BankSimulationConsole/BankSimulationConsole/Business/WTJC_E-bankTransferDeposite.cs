using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

using IBusiness;
using CommonTools;
using Entities.BllModels;
using Entities;
using IDataAccess;
using System.Configuration;

namespace Business
{
    /// <summary>
    /// 网厅缴存--网银转账缴存对账
    /// </summary>
    public class WTJC_E_bankTransferDeposite:GjjBusinessSuper
    {
        /// <summary>
        /// 业务实体
        /// </summary>
        WtWyzhDzModel wtWyzhDz = new WtWyzhDzModel();
        
         /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            //解析请求报文
            byte[] transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            byte[] BbankCount = BusinessTools.SubBytesArray(recvBytes, 4, 30);
            byte[] bDateStart = BusinessTools.SubBytesArray(recvBytes, 34, 8);
            byte[] bDateEnd = BusinessTools.SubBytesArray(recvBytes, 42, 8);

            byte[] sumRecords = BusinessTools.SubBytesArray(recvBytes, 50, 6);
            byte[] sumMoney = BusinessTools.SubBytesArray(recvBytes, 56, 16);

            wtWyzhDz.Jym = Encoding.Default.GetString(transcationCode).TrimEnd();
            wtWyzhDz.Yhzh = Encoding.Default.GetString(BbankCount).TrimEnd();
            wtWyzhDz.Qsrq = Encoding.Default.GetString(bDateStart).TrimEnd();
            wtWyzhDz.Zzrq = Encoding.Default.GetString(bDateEnd).TrimEnd();
            wtWyzhDz.Zbs = Encoding.Default.GetString(sumRecords).TrimEnd();
            wtWyzhDz.Zje = Encoding.Default.GetString(sumMoney).TrimEnd();

            string result = "";
            string fileName = "";
            Thread.Sleep(2000);
            WTJC_WangyinZhuanzhangBusiness(whichBank, wtWyzhDz, out fileName);
            result = WTJC_WangyinZhuanzhangMessage(wtWyzhDz, fileName);

            LogHelper.WriteLogInfo("网厅缴存--网银转账缴存对账", "成功");
            return Encoding.Default.GetBytes(result);
        }

        /// <summary>
        /// 网银转账缴存对账业务处理
        /// </summary>
        /// <param name="whichBank"></param>
        /// <param name="batchCodeStart"></param>
        /// <param name="batchCodeEnd"></param>
        /// <param name="jgm"></param>
        /// <param name="sumRecords"></param>
        /// <param name="sumMoney"></param>
        /// <param name="outFileName"></param>
        private void WTJC_WangyinZhuanzhangBusiness(string whichBank, WtWyzhDzModel wtWyzhDz, out string outFileName)
        {
            string fileName = "";
            fileName += "11";//机构码
            fileName += "G501";
            fileName += "_W";

            DateTime dt = new DateTime();
            string strDate = dt.ToString("yyyyMMdd");

            fileName += strDate;
            fileName += ".";
            fileName += "380910";//6位银行代号

            outFileName = fileName;

            string filePath = BusinessTools.GetFilePath(whichBank) + fileName;//文件的完整路径


            IDB2Operation iDB2Operation = BusinessHelper.GetDb2Connection();
            List<ZbmxzEntity> list = iDB2Operation.GetZbmxzByJyrq(wtWyzhDz.Qsrq,wtWyzhDz.Zzrq);

            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("gb2312")))
            {
                string summaryLine = string.Empty;
                summaryLine += wtWyzhDz.Yhzh;
                summaryLine += ",";
                summaryLine += strDate;
                summaryLine += ",";
                summaryLine += wtWyzhDz.Zje;
                summaryLine += ",";
                summaryLine += wtWyzhDz.Zbs;
                summaryLine += ",";
                sw.WriteLine(summaryLine);//汇总行
            }

            //明细行
            for (int i = 1; i <= list.Count; i++)
            {
                string detailLine = string.Empty;
                detailLine += i.ToString();
                detailLine += ",";
                detailLine += list[i].Jyrq;
                detailLine += ",";
                detailLine += list[i].Jysj;
                detailLine += ",";
                detailLine += BusinessTools.GenerateBatchCode("110000000", i);//批次号
                detailLine += ",";
                detailLine += BusinessTools.GenerateName("李", i);
                detailLine += ",";
                detailLine += list[i].Zh;
                detailLine += ",";
                detailLine += list[i].Fse;
                detailLine += ",";
                detailLine += list[i].Yhls;//银行流水
                detailLine += ",";
                detailLine += list[i].Jdbz;//记账标志
                detailLine += ",";
                detailLine += list[i].Yhls;//备注中添写银行流水号
                detailLine += ",";

                using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("gb2312")))
                {
                    sw.WriteLine(detailLine);
                }
            }
        }

        /// <summary>
        /// 网银转账缴存对账响应报文
        /// </summary>
        /// <param name="transcationCode"></param>
        /// <param name="batchCodeStart"></param>
        /// <param name="batchCodeEnd"></param>
        /// <param name="sumRecords"></param>
        /// <param name="sumMoney"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string WTJC_WangyinZhuanzhangMessage(WtWyzhDzModel wtWyzhDz,string fileName)
        {
            string s = "";
            byte[] length = new byte[4];
            byte[] bTranCode = new byte[4];
            byte[] bRetuCode = new byte[4];
            byte[] bRetuValueInfo = new byte[60];
            byte[] bFileName = new byte[30];
            byte[] bSumRecords = new byte[6];
            byte[] bSumMoney = new byte[16];
            byte[] bBankCount = new byte[30];

            BusinessTools.SetByteArray(length, "0150");
            BusinessTools.SetByteArray(bTranCode, "3018");
            BusinessTools.SetByteArray(bRetuCode, "0000");

            BusinessTools.InitializeByteArray(bRetuValueInfo, 60);
            BusinessTools.InitializeByteArray(bBankCount, 30);
            BusinessTools.SetByteArray(bBankCount, wtWyzhDz.Yhzh);

            BusinessTools.InitializeByteArray(bFileName, 60);
            BusinessTools.SetByteArray(bFileName, fileName);
            BusinessTools.InitializeByteArray(bSumRecords, 6);
            BusinessTools.SetByteArray(bSumRecords, wtWyzhDz.Zbs);
            BusinessTools.InitializeByteArray(bSumMoney, 16);
            BusinessTools.SetByteArray(bSumMoney, wtWyzhDz.Zje);

            s += Encoding.Default.GetString(length);
            s += Encoding.Default.GetString(bTranCode);
            s += Encoding.Default.GetString(bRetuCode);
            s += Encoding.Default.GetString(bRetuValueInfo);
            s += Encoding.Default.GetString(bBankCount);
            s += Encoding.Default.GetString(bFileName);
            s += Encoding.Default.GetString(bSumRecords);
            s += Encoding.Default.GetString(bSumMoney);

            return s;
        }
    }
}
