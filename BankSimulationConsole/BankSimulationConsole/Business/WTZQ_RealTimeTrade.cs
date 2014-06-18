using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Data;

using IBusiness;
using CommonTools;
using Entities;
using Entities.BllModels;
using IDataAccess;

namespace Business
{
    /// <summary>
    /// 网厅支取——实时交易日终对账
    /// </summary>
    public class WTZQ_RealTimeTrade:GjjBusinessSuper
    {
        /// <summary>
        /// 网厅支取--实时交易日终对账实体
        /// </summary>
        WtzqSsjyModel wtzqssjy = new WtzqSsjyModel();

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

            wtzqssjy.Jym = Encoding.Default.GetString(transcationCode).TrimEnd();
            wtzqssjy.Kspch = Encoding.Default.GetString(batchCodeStart).TrimEnd();
            wtzqssjy.Jspch = Encoding.Default.GetString(batchCodeEnd).TrimEnd();
            wtzqssjy.Jgm = Encoding.Default.GetString(jgm).TrimEnd();
            wtzqssjy.Zbs = Encoding.Default.GetString(sumRecords).TrimEnd();
            wtzqssjy.Zje = Encoding.Default.GetString(sumMoney).TrimEnd();

            string fileName = "";
            Thread.Sleep(2000);
            WTZQ_ShishiJiaoyiBusiness(whichBank, wtzqssjy, out fileName);
            string result = WTZQ_ShishiJiaoyiRizhongDuizhangMessage(wtzqssjy, fileName);

            LogHelper.WriteLogInfo("网厅支取——实时交易日终对账", "成功");
            return Encoding.Default.GetBytes(result);
        }

        /// <summary>
        /// 网厅支取--实时交易日终对账业务处理，产生明细文件
        /// </summary>
        /// <param name="whichBank">行别</param>
        /// <param name="wtzqssjy">业务实体</param>
        /// <param name="outFileName">产生的明细文件名称</param>
        private void WTZQ_ShishiJiaoyiBusiness(string whichBank, WtzqSsjyModel wtzqssjy, out string outFileName)
        {
            List<ZbmxzEntity> zbmxList = new List<ZbmxzEntity>();
            IDB2Operation iDB2Operation = BusinessHelper.GetDb2Connection();
            zbmxList = iDB2Operation.GetZbmxzByPch(wtzqssjy.Kspch, wtzqssjy.Jspch);

            string strDate = DateTime.Now.ToShortDateString();
            string fileName = "";
            fileName += wtzqssjy.Jgm;
            fileName += "Z";//支取
            fileName += "_W";
            fileName += strDate;
            fileName += ".";
            fileName += "380910";//6位银行代号

            outFileName = fileName;

            string filePath = BusinessTools.GetFilePath(whichBank) + fileName;//文件的完整路径
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("gb2312")))
            {
                string summaryLine = string.Empty;
                summaryLine += wtzqssjy.Jgm;
                summaryLine += ",";
                summaryLine += strDate;
                summaryLine += ",";
                summaryLine += wtzqssjy.Zje;
                summaryLine += ",";
                summaryLine += wtzqssjy.Zbs;
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
        /// 网厅支取--实时交易日终对账响应报文
        /// </summary>
        /// <param name="transcationCode"></param>
        /// <param name="batchCodeStart"></param>
        /// <param name="batchCodeEnd"></param>
        /// <param name="jgm"></param>
        /// <param name="sumRecords"></param>
        /// <param name="sumMoney"></param>
        /// <returns></returns>
        private string WTZQ_ShishiJiaoyiRizhongDuizhangMessage(WtzqSsjyModel wtzqssjy, string fileName)
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
            BusinessTools.SetByteArray(bTranCode, "3007");
            BusinessTools.SetByteArray(bRetuCode, "0000");
            BusinessTools.SetByteArray(bJgm, wtzqssjy.Jgm);

            BusinessTools.InitializeByteArray(bRetuValueInfo, 60);

            BusinessTools.InitializeByteArray(bFileName, 60);
            BusinessTools.SetByteArray(bFileName, fileName);
            BusinessTools.InitializeByteArray(bSumRecords, 6);
            BusinessTools.SetByteArray(bSumRecords, wtzqssjy.Zbs);
            BusinessTools.InitializeByteArray(bSumMoney, 16);
            BusinessTools.SetByteArray(bSumMoney, wtzqssjy.Zje);

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
