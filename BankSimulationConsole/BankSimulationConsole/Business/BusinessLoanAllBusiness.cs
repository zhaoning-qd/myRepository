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
    /// 商业贷款
    /// </summary>
    public class BusinessLoanAllBusiness:GjjBusinessSuper
    {
        /// <summary>
        /// 处理业务
        /// </summary>
        public override byte[] HandleBusiness(byte[] recvBytes, string whichBank)
        {
            string s = "";
            Thread.Sleep(3000);
            s = BusinessLoanAllMessage(recvBytes);
            return Encoding.Default.GetBytes(s);
        }

        /// <summary>
        /// 商业贷款签约，撤销、查询
        /// </summary>
        /// <param name="recvBytes"></param>
        /// <returns></returns>
        private string BusinessLoanAllMessage(byte[] recvBytes)
        {
            byte[] transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            byte[] hth = BusinessTools.SubBytesArray(recvBytes, 4, 30);
            byte[] sfz = BusinessTools.SubBytesArray(recvBytes, 34, 18);
            byte[] xm = BusinessTools.SubBytesArray(recvBytes, 52, 20);
            byte[] bz = BusinessTools.SubBytesArray(recvBytes, 72, 1);
            byte[] beiz = BusinessTools.SubBytesArray(recvBytes, 73, 30);

            string result = "";

            string businessType = Encoding.Default.GetString(bz);
            switch (businessType)
            {
                case "A"://签约
                    result = BusinessLoanSign(Encoding.Default.GetString(transcationCode),
                                              Encoding.Default.GetString(hth),
                                              Encoding.Default.GetString(sfz),
                                              Encoding.Default.GetString(xm),
                                              Encoding.Default.GetString(beiz), businessType);
                    LogHelper.WriteLogInfo("商业贷款","签约成功");
                    break;
                case "D"://撤销
                    result = BusinessLoanCancel(Encoding.Default.GetString(transcationCode));
                    LogHelper.WriteLogInfo("商业贷款", "撤销成功");
                    break;
                case "Q"://查询
                    result = BusinessLoanSign(Encoding.Default.GetString(transcationCode),
                                              Encoding.Default.GetString(hth),
                                              Encoding.Default.GetString(sfz),
                                              Encoding.Default.GetString(xm),
                                              Encoding.Default.GetString(beiz), businessType);
                    LogHelper.WriteLogInfo("商业贷款", "查询成功");
                    break;
            }

            return result;
        }

        /// <summary>
        /// 用于商业贷款签约和查询返回报文的产生
        /// </summary>
        /// <param name="transcationCode"></param>
        /// <param name="hth"></param>
        /// <param name="sfz"></param>
        /// <param name="xm"></param>
        /// <param name="beiz"></param>
        /// <returns></returns>
        private string BusinessLoanSign(string transcationCode, string hth, string sfz, string xm,
            string beiz, string type)
        {
            string result = "";
            string length = "0229";
            string returnCode = "0000";

            string hkzh = "12345657878565";//还款账号
            byte[] bHkzh = new byte[30];
            BusinessTools.InitializeByteArray(bHkzh, 30);
            BusinessTools.SetByteArray(bHkzh, hkzh);
            string sHkzh = Encoding.Default.GetString(bHkzh);

            string hkfs = "1";//还款方式
            string gfrq = "20010912";//购房日期
            string fkrq = "20020913";//放款日期
            string dqrq = "20200912";//到期日期
            string dkqx = "240";//贷款期限

            string dkll = "4.5000";//贷款利率
            byte[] bDkll = new byte[8];
            BusinessTools.InitializeByteArray(bDkll, 8);
            BusinessTools.SetByteArray(bDkll, dkll);
            string sDkll = Encoding.Default.GetString(bDkll);

            string yhkrq = "20020920";//应还款日期
            string hkr = "20";//还款日

            string yhkje = "3000.00";//月还款金额
            byte[] bYhkje = new byte[10];
            BusinessTools.InitializeByteArray(bYhkje, 10);
            BusinessTools.SetByteArray(bYhkje, yhkje);
            string sYhkje = Encoding.Default.GetString(bYhkje);

            string syqs = "";//剩余期数
            if (type == "A")
            {
                syqs = "240";
            }
            if (type == "Q")
            {
                syqs = "180";
            }

            string jkje = "360000.00";//借款金额
            byte[] bJkje = new byte[12];
            BusinessTools.InitializeByteArray(bJkje, 12);
            BusinessTools.SetByteArray(bJkje, jkje);
            string sJkje = Encoding.Default.GetString(bJkje);

            string sybj = "";//剩余本金
            if (type == "A")
            {
                sybj = "360000.00";
            }
            if (type == "Q")
            {
                sybj = "240000.00";
            }
            byte[] bSybj = new byte[12];
            BusinessTools.InitializeByteArray(bSybj, 12);
            BusinessTools.SetByteArray(bSybj, sybj);
            string sSybj = Encoding.Default.GetString(bSybj);

            string fjze = "1000000.00";//房价总额
            byte[] bFjze = new byte[12];
            BusinessTools.InitializeByteArray(bFjze, 12);
            BusinessTools.SetByteArray(bFjze, fjze);
            string sFjze = Encoding.Default.GetString(bFjze);

            string fwdz = "青岛市杭州路131号1号楼3单元705";//房屋地址
            byte[] bFwdz = new byte[80];
            BusinessTools.InitializeByteArray(bFwdz, 80);
            BusinessTools.SetByteArray(bFwdz, fwdz);
            string sFwdz = Encoding.Default.GetString(bFwdz);

            string jzmj = "98.0000";//建筑面积
            byte[] bJzmj = new byte[8];
            BusinessTools.InitializeByteArray(bJzmj, 8);
            BusinessTools.SetByteArray(bJzmj, jzmj);
            string sJzmj = Encoding.Default.GetString(bJzmj);

            result += length;
            result += transcationCode;
            result += returnCode;
            result += hth;
            result += sfz;
            result += xm;
            result += sHkzh;
            result += hkfs;
            result += gfrq;
            result += fkrq;
            result += dqrq;
            result += dkqx;
            result += sDkll;
            result += yhkrq;
            result += hkr;
            result += sYhkje;
            result += syqs;
            result += sJkje;
            result += sSybj;
            result += sFjze;
            result += sFwdz;
            result += sJzmj;

            return result;
        }

        // <summary>
        /// 用于商业贷款撤销返回报文的产生
        /// </summary>
        /// <param name="transcationCode"></param>
        /// <param name="hth"></param>
        /// <param name="sfz"></param>
        /// <param name="xm"></param>
        /// <param name="beiz"></param>
        /// <returns></returns>
        private string BusinessLoanCancel(string transcationCode)
        {
            string result = "";
            result += transcationCode;
            result += "0000";

            return result;
        }


        /// <summary>
        /// 商业贷款明细发送
        /// </summary>
        /// <returns></returns>
        public byte[] BusinessLoanDetailSend(string yhdh, string filePath)
        {
            //生成报盘文件
            //文件名
            string fileName = yhdh;
            fileName += "_";
            DateTime dt = DateTime.Now;
            string strDate = dt.ToString("yyyyMMdd");
            fileName += strDate;

            //生成具体文件内容
            FileStream fs = new FileStream(filePath + fileName, FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("gb2312")))
            {
                sw.WriteLine("H~5~10000089.50~10000089.50~~~~");
                string s1 = "M~4564656576757~123456789009876543~45465654646~21~20120912~20120912~35~5000~270~250000~预留1~预留2~";
                string s2 = "M~4564656576758~345456789009876543~45567654646~21~20120913~20120913~35~4000~260~240000~预留1~预留2~";
                string s3 = "M~4564656576759~654456789009876543~23465654646~21~20120914~20120914~35~3000~250~230000~预留1~预留2~";
                string s4 = "M~4564656576750~876456789009876543~76845654646~21~20120915~20120915~35~2000~240~220000~预留1~预留2~";
                string s5 = "M~4564656576751~556456789009876543~16435654646~21~20120916~20120916~35~2500~230~210000~预留1~预留2~";
                sw.WriteLine(s1);
                sw.WriteLine(s2);
                sw.WriteLine(s3);
                sw.WriteLine(s4);
                sw.WriteLine(s5);
            }

            Console.WriteLine("报盘文件生成");

            string s = "";
            s = BusinessLoanDetailSendMessage(fileName);
            return Encoding.Default.GetBytes(s);

        }

        /// <summary>
        /// 商业贷款明细发送
        /// </summary>
        /// <returns></returns>
        public string BusinessLoanDetailSendMessage(string fileName)
        {
            string s = "";
            s += "0034";
            s += "4001";

            byte[] bFileName = new byte[30];
            BusinessTools.InitializeByteArray(bFileName, 30);
            BusinessTools.SetByteArray(bFileName, fileName);

            s += Encoding.Default.GetString(bFileName);

            return s;
        }
    }
}
