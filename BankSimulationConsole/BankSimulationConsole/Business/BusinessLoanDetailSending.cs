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
    /// 贷款明细发送
    /// </summary>
    public class BusinessLoanDetailSending : BusinessLaunchedByBankSuper
    {
        /// <summary>
        /// 业务处理
        /// </summary>
        /// <returns></returns>
        public override byte[] HandleBusiness()
        {
            return BusinessLoanDetailSend("21010", "D:\\gjj_file\\");
        }

        /// <summary>
        /// 商业贷款明细发送
        /// </summary>
        /// <returns></returns>
        private byte[] BusinessLoanDetailSend(string yhdh, string filePath)
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

            LogHelper.WriteLogInfo("商业贷款明细发送", "消息发送成功");
            return Encoding.Default.GetBytes(s);

        }

        /// <summary>
        /// 商业贷款明细发送
        /// </summary>
        /// <returns></returns>
        private string BusinessLoanDetailSendMessage(string fileName)
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
