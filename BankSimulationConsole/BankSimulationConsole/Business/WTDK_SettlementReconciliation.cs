using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

using IBusiness;
using CommonTools;
using Entities;
using Entities.BllModels;
using IDataAccess;

namespace Business
{

    /// <summary>
    /// 网厅贷款--结算交易日终对账
    /// </summary>
    public class WTDK_SettlementReconciliation:GjjBusinessSuper
    {
        /// <summary>
        /// 业务实体
        /// </summary>
        WtdkJsjyRzdzModel wtdkJsjyRzdz = new WtdkJsjyRzdzModel();

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

            wtdkJsjyRzdz.Jym = Encoding.Default.GetString(transcationCode).TrimEnd();
            wtdkJsjyRzdz.Kspch = Encoding.Default.GetString(batchCodeStart).TrimEnd();
            wtdkJsjyRzdz.Jspch = Encoding.Default.GetString(batchCodeEnd).TrimEnd();
            wtdkJsjyRzdz.Jgm = Encoding.Default.GetString(jgm).TrimEnd();
            wtdkJsjyRzdz.Zbs = Encoding.Default.GetString(sumRecords).TrimEnd();
            wtdkJsjyRzdz.Zje = Encoding.Default.GetString(sumMoney).TrimEnd();

            string result = "";
            string fileName = "";
            Thread.Sleep(2000);
            WT_DaikuanJiesuanBusiness(whichBank, wtdkJsjyRzdz, out fileName);
            result = WT_DaikuanJiesuanDuizhangMessage(wtdkJsjyRzdz, fileName);

            LogHelper.WriteLogInfo("网厅贷款--结算交易日终对账", "成功");
            return Encoding.Default.GetBytes(result);
        }

        private void WT_DaikuanJiesuanBusiness(string whichBank, WtdkJsjyRzdzModel wtdkJsjyRzdz, out string outFileName)
        {
            List<ZbmxzEntity> zbmxList = new List<ZbmxzEntity>();
            IDB2Operation iDB2Operation = BusinessHelper.GetDb2Connection();
            zbmxList = iDB2Operation.GetZbmxzByPch(wtdkJsjyRzdz.Kspch, wtdkJsjyRzdz.Jspch);

            string fileName = "";
            fileName += wtdkJsjyRzdz.Jgm;
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
                summaryLine += wtdkJsjyRzdz.Jgm;
                summaryLine += ",";
                summaryLine += strDate;
                summaryLine += ",";
                summaryLine += wtdkJsjyRzdz.Zje;
                summaryLine += ",";
                summaryLine += wtdkJsjyRzdz.Zbs;
                summaryLine += ",";
                sw.WriteLine(summaryLine);//汇总行
            }

            //明细行
            for (int i = 1; i <= zbmxList.Count; i++)
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
                detailLine += zbmxList[i].Pjhm;//批次号
                detailLine += ",";
                detailLine += BusinessTools.GenerateName("李", i);
                detailLine += ",";
                detailLine += zbmxList[i].Zh;
                detailLine += ",";
                detailLine += zbmxList[i].Fse;
                detailLine += ",";
                detailLine += zbmxList[i].Yhls;//银行流水
                detailLine += ",";
                detailLine += zbmxList[i].Jdbz;//记账标志
                detailLine += ",";
                detailLine += zbmxList[i].Yhls;//备注中添写银行流水号
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
        public string WT_DaikuanJiesuanDuizhangMessage(WtdkJsjyRzdzModel wtdkJsjyRzdz,string fileName)
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
            BusinessTools.SetByteArray(bJgm, wtdkJsjyRzdz.Jgm);

            BusinessTools.InitializeByteArray(bRetuValueInfo, 60);

            BusinessTools.InitializeByteArray(bFileName, 60);
            BusinessTools.SetByteArray(bFileName, fileName);
            BusinessTools.InitializeByteArray(bSumRecords, 6);
            BusinessTools.SetByteArray(bSumRecords, wtdkJsjyRzdz.Zbs);
            BusinessTools.InitializeByteArray(bSumMoney, 16);
            BusinessTools.SetByteArray(bSumMoney, wtdkJsjyRzdz.Zje);

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
