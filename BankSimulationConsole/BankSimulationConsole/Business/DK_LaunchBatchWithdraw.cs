using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBusiness;
using CommonTools;
using System.Threading;
using System.IO;
using IDataAccess;
using System.Configuration;

namespace Business
{
    /// <summary>
    /// 贷款业务--银行批量收回发起
    /// </summary>
    public class DK_LaunchBatchWithdraw:MsgFirstBusinessSuper
    {
        /// <summary>
        /// 产生立即响应消息
        /// </summary>
        /// <param name="recvBytes"></param>
        /// <returns></returns>
        public override byte[] GenerateResponseRealTimeMsg(byte[] recvBytes)
        {
            string s = "";
            byte[] transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            byte[] batchCode = BusinessTools.SubBytesArray(recvBytes, 4, 20);
            byte[] sumRecords = BusinessTools.SubBytesArray(recvBytes, 24, 6);
            byte[] sumMoney = BusinessTools.SubBytesArray(recvBytes, 30, 12);
            byte[] fileName = BusinessTools.SubBytesArray(recvBytes, 42, 30);
            byte[] returnCode = Encoding.Default.GetBytes("0000");

            s += "0028";
            s += Encoding.Default.GetString(transcationCode);
            s += Encoding.Default.GetString(returnCode);
            s += Encoding.Default.GetString(batchCode);

            LogHelper.WriteLogInfo("贷款业务--银行批量收回发起", "成功发送即时响应消息");
            return Encoding.Default.GetBytes(s);
        }

        /// <summary>
        /// 处理业务
        /// </summary>
        public override bool HandleBusiness(byte[] recvBytes, string whichBank)
        {
            //解析报文;
            byte[] transcationCode = BusinessTools.SubBytesArray(recvBytes, 0, 4);
            byte[] batchCode = BusinessTools.SubBytesArray(recvBytes, 4, 20);
            byte[] sumRecords = BusinessTools.SubBytesArray(recvBytes, 24, 6);
            byte[] sumMoney = BusinessTools.SubBytesArray(recvBytes, 30, 12);
            byte[] fileName = BusinessTools.SubBytesArray(recvBytes, 42, 30);////DKS20101108H.010001.00000001;
            string yhdh = "";//银行代号;
            try
            {
                yhdh = Encoding.Default.GetString(fileName).Substring(13, 6);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //开始处理具体业务;
            string fileGenerated;
            bool isSuccess = LoanBatchWithDrawDetail(whichBank, transcationCode, batchCode, sumRecords, sumMoney,
                fileName, out fileGenerated);
            if (isSuccess)
            {
                LogHelper.WriteLogInfo("贷款批量收回", "成功");
                return true;
            }
            else
            {
                LogHelper.WriteLogInfo("贷款批量收回", "失败");
                return false;
            }
        }

        /// <summary>
        /// 业务处理详情
        /// </summary>
        /// <param name="whichBank"></param>
        /// <param name="transcationCode"></param>
        /// <param name="batchCode"></param>
        /// <param name="sumRecords"></param>
        /// <param name="sumMoney"></param>
        /// <param name="fileName"></param>
        /// <param name="fileGenerated"></param>
        /// <returns></returns>
        private static bool LoanBatchWithDrawDetail(string whichBank, byte[] transcationCode, byte[] batchCode,
            byte[] sumRecords, byte[] sumMoney, byte[] fileName, out string fileGenerated)
        {
            //具体处理过程;
            Thread.Sleep(3000);
            //从ftp服务器取文件,本程序中是从本机读取;
            string fileFromPath = BusinessTools.GetFilePath(whichBank); ;
            string inputLine = "";
            StringBuilder outputLine;

            DateTime dt = DateTime.Now;
            string strDate = dt.ToString("yyyyMMdd");
            string tail = Encoding.Default.GetString(fileName).Substring(3);
            string outFile = "DKR" + tail;//返回文件的名称 ;
            fileGenerated = outFile;
            string filePath = fileFromPath + outFile;

            try
            {
                using (StreamReader sr = new StreamReader(fileFromPath + Encoding.Default.GetString(fileName), Encoding.GetEncoding("gb2312")))
                {
                    inputLine = sr.ReadLine();//读取第一行汇总数据;
                    string[] s = inputLine.Split(new char[] { '~' });
                    s[3] = s[2];
                    inputLine = s[0] + "~";
                    inputLine += s[1];
                    inputLine += "~";
                    inputLine += s[2];
                    inputLine += "~";
                    inputLine += s[3];
                    inputLine += "~";
                    FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("gb2312")))
                    {
                        sw.WriteLine(inputLine);
                    }

                    for (int i = 1; i <= Convert.ToInt32(Encoding.Default.GetString(sumRecords)); i++)
                    {
                        inputLine = sr.ReadLine();

                        string[] inputArray = inputLine.Split(new char[] { '~' });

                        outputLine = new StringBuilder();
                        outputLine.Append("M~");
                        outputLine.Append(inputArray[1]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[2]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[3]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[3]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[5]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[6]);
                        outputLine.Append("~");
                        outputLine.Append("0000");//银行扣款的状态标志;
                        outputLine.Append("~");

                        outputLine.Append(inputArray[8]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[9]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[10]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[11]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[12]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[13]);
                        outputLine.Append("~");


                        using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("gb2312")))
                        {
                            sw.WriteLine(outputLine.ToString());
                        }
                    }
                }
                Console.WriteLine("文件处理成功");

                //模拟前置机动作：更新djplzxzf的zt字段;
                string command = "update djplzxzf set zt='3' where djhm='" + Encoding.Default.GetString(batchCode).Replace(" ", "") + "'";
                Console.WriteLine("开始更新数据库表");

                string assemblyName = "DataAccess";
                string namespaceName = "DataAccess";
                string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
                IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

                bool bStatus = iDB2Operation.ExecuteDB2Update(command);
                if (bStatus)
                {
                    Console.WriteLine("更新djplzxzf状态成功");
                    LogHelper.WriteLogInfo("贷款批量收回", "更新djplzxzf状态成功");
                }
                else
                {
                    Console.WriteLine("更新djplzxzf状态失败");
                    LogHelper.WriteLogError("贷款批量收回", "更新djplzxzf状态失败");

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                fileGenerated = "";
                return false;
            }
            return true;
        }
    }
}
