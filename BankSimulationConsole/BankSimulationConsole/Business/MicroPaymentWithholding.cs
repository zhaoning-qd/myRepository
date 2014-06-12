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
using Entities;

namespace Business
{
    /// <summary>
    /// 小额支付代扣
    /// </summary>
    public class MicroPaymentWithholding : MsgFirstBusinessSuper
    {
        /// <summary>
        /// 产生立即响应消息
        /// </summary>
        /// <param name="recvBytes"></param>
        /// <returns></returns>
        public override byte[] GenerateResponseRealTimeMsg(byte[] recvBytes)
        {
            string s = "0028";
            string transcationCode, batchCode, returnCode;
            transcationCode = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 0, 4));
            batchCode = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 4, 20));
            returnCode = "0000";

            s += transcationCode;
            s += returnCode;
            s += batchCode;

            LogHelper.WriteLogInfo("小额支付代扣", "发送即时响应消息成功");
            return Encoding.Default.GetBytes(s);
        }

        /// <summary>
        /// 处理业务
        /// </summary>
        public override bool HandleBusiness(byte[] recvBytes, string whichBank)
        {
            //解析报文;
            string transcationCode, batchCode, sumCount, sumMoney, fileName;
            transcationCode = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 0, 4));
            batchCode = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 4, 20));
            sumCount = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 24, 6));
            sumMoney = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 30, 12));
            fileName = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 42, 30));
            string yhdh = "";
            try
            {
                yhdh = fileName.Substring(24);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("银行代号：{0}", yhdh);
            if (yhdh.Length != 6)
            {
                Console.WriteLine("获取银行代号失败");
                return false;
            }

            string fileGenerated;
            bool isSuccess = XiaoEZhiFuDaiKouDetail(whichBank, batchCode, sumCount, sumMoney, yhdh, fileName,
                out fileGenerated);
            if (isSuccess)
            {
                LogHelper.WriteLogInfo("小额支付代扣业务", "成功");
                return true;
            }
            else
            {
                LogHelper.WriteLogInfo("小额支付代扣业务", "失败");
                return false;
            }
        }

        /// <summary>
        /// 小额支付代扣处理详情;
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool XiaoEZhiFuDaiKouDetail(string whichBank, string picihao, string sum_count,
            string sum_money, string yhdh, string fileName, out string fileGenerated)
        {
            //具体处理过程;
            Thread.Sleep(3000);
            //从ftp服务器取文件,本程序中是从本机读取;
            string fileFromPath = BusinessTools.GetFilePath(whichBank);
            string inputLine = "";
            StringBuilder outputLine;

            DateTime dt = DateTime.Now;
            string strDate = dt.ToString("yyyyMMdd");
            string tail = fileName.Substring(4);
            string outFile = "HRB_" + tail;//返回文件的名称 ;
            fileGenerated = outFile;
            string filePath = fileFromPath + outFile;

            try
            {
                using (StreamReader sr = new StreamReader(fileFromPath + fileName, Encoding.GetEncoding("gb2312")))
                {
                    inputLine = sr.ReadLine();//读取第一行汇总数据;
                    FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("gb2312")))
                    {
                        sw.WriteLine(inputLine);
                    }
                    for (int i = 1; i <= Convert.ToInt32(sum_count); i++)
                    {
                        inputLine = sr.ReadLine();

                        string[] inputArray = inputLine.Split(new char[] { '~' });
                        //string skr_khyh = inputArray[1];//收款人开户银行;
                        //string skr_zh = inputArray[2];//收款人账号;
                        //string skr_mc = inputArray[3];//收款人名称;
                        //string fkr_khyh = inputArray[4];//付款人开户银行;
                        //string fkr_zh = inputArray[5];//付款人账号;
                        //string fkr_mc = inputArray[6];//付款人名称;
                        //string jine = inputArray[7];//金额;
                        //string xybh = inputArray[8];//协议编号;
                        //string djhm = inputArray[9];//单据号;
                        //string yhjgdm = inputArray[10];//银行机构代码;
                        //string beiz = inputArray[11];//备注;

                        //扣款处理;
                        //完成...;
                        //Random rnd = new Random();
                        string kkzt = BatchWithHolding(1);//扣款状态,全部返回成功;
                        string kkxx = "0" + kkzt;//扣款信息;
                        //生成银行流水号;
                        string yhlsh = "";
                        yhlsh += strDate;
                        if (i < 10)
                        {
                            yhlsh += "0";
                        }
                        yhlsh += i.ToString();

                        outputLine = new StringBuilder();
                        outputLine.Append("M~");
                        outputLine.Append(inputArray[1]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[2]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[3]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[4]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[5]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[6]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[7]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[8]);
                        outputLine.Append("~");
                        outputLine.Append(kkzt);
                        outputLine.Append("~");
                        outputLine.Append(kkxx);
                        outputLine.Append("~");
                        outputLine.Append(i.ToString());//汇划报文顺序号
                        outputLine.Append("~");
                        outputLine.Append(yhlsh);
                        outputLine.Append("~");
                        outputLine.Append(strDate);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[9]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[10]);
                        outputLine.Append("~");
                        outputLine.Append(inputArray[11]);
                        outputLine.Append("~");

                        using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("gb2312")))
                        {
                            sw.WriteLine(outputLine.ToString());
                        }

                        //更新账表分户账和账表明细账
                       // UpateZbfhzAndZbmxz(inputArray);
                    }
                }
                Console.WriteLine("文件处理成功");

                //模拟前置机动作：更新djplzxzf的zt字段;
                string command = "update djplzxzf set zt='3' where djhm='" + picihao.Replace(" ", "") + "'";
                Console.WriteLine("开始更新数据库表");

                string assemblyName = "DataAccess";
                string namespaceName = "DataAccess";
                string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
                IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);
                
                bool bStatus = iDB2Operation.ExecuteDB2Update(command);
                if (bStatus)
                {
                    Console.WriteLine("更新djplzxzf状态成功");
                    LogHelper.WriteLogInfo("小额支付批量代扣", "更新djplzxzf状态成功");
                }
                else
                {
                    Console.WriteLine("更新djplzxzf状态失败");
                    LogHelper.WriteLogError("小额支付批量代扣", "更新djplzxzf状态失败");

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

        /// <summary>
        /// 返回报文
        /// </summary>
        /// <param name="recvBytes"></param>
        /// <param name="fileGenerated"></param>
        /// <returns></returns>
        public byte[] XiaoEZhiFuDaiDouOffLineMessage(byte[] recvBytes, string fileGenerated)
        {
            string s = "0072";
            //解析报文;
            string transcationCode, batchCode, sumCount, sumMoney;
            transcationCode = "2001";
            batchCode = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 4, 20));
            sumCount = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 24, 6));
            sumMoney = Encoding.Default.GetString(BusinessTools.SubBytesArray(recvBytes, 30, 12));

            byte[] fileName = new byte[30];
            for (int i = 0; i < 30; i++)
            {
                if (i <= fileGenerated.Length)
                {
                    fileName[i] = Convert.ToByte(fileGenerated[i]);
                }
                else
                {
                    fileName[i] = Convert.ToByte(" ");
                }

            }

            s += transcationCode;
            s += batchCode;
            s += sumCount;
            s += sumMoney;
            s += fileGenerated;

            return Encoding.Default.GetBytes(s);
        }

        /// <summary>
        /// 批量扣款结果;
        /// </summary>
        /// <param name="zt"></param>
        /// <returns></returns>
        private string BatchWithHolding(int zt)
        {
            //扣款，返回结果;
            switch (zt)
            {
                case 1://成功;
                    return "00";
                case 2://账号不存在;
                    return "01";
                case 3://账户名不存在;
                    return "02";
                case 4://账户余额不足支付;
                    return "03";
                case 5://账户密码错误;
                    return "10";
                case 6://账户状态错误;
                    return "11";
                case 7://业务已撤销;
                    return "20";
                case 8://其它错误;
                    return "90";
                case 9://回执超时;
                    return "0b";
                default:
                    return "";

            }
        }


        /// <summary>
        /// 查询zbmxz中某账号的笔数
        /// </summary>
        /// <returns></returns>
        private int GetCountByZh(ZbmxzEntity zbmxz)
        {
            string assemblyName = "DataAccess";
            string namespaceName = "DataAccess";
            string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
            IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

            return iDB2Operation.ExecuteCountQuery(zbmxz.ToCountStringByZh());
        }


        /// <summary>
        ///更新zbfhz和zbmxz
        /// </summary>
        /// <param name="lineArray"></param>
        /// <returns></returns>
        private bool UpateZbfhzAndZbmxz(string[] lineArray)
        {
            //账表明细账
            bool zt1,zt2;
            ZbmxzEntity zbmxz = new ZbmxzEntity();
            zbmxz.Zh = lineArray[5];
            int iBs = this.GetCountByZh(zbmxz);
            if (iBs == -1)
            {
                Console.WriteLine("查询账号{0}对应的笔数出错", zbmxz.Zh);
                return false;
            }
            else
            {
                zbmxz.Bc = (iBs + 1).ToString();
                zbmxz.Jyrq = DateTime.Now.ToShortDateString();
                zbmxz.Jysj = DateTime.Now.ToLongTimeString();
                zbmxz.Fse = lineArray[3];
                zbmxz.Ye = "200000";
                Random radom = new Random();
                zbmxz.Yhls = BusinessTools.GenerateLongBankSerialNum(radom.Next(99));
                zbmxz.Pjhm = lineArray[10];
                zbmxz.Jdbz = "2";
                zbmxz.Ywlx = "1";
                zbmxz.Dfzh = lineArray[2];
                zbmxz.Dfhm = lineArray[3];
                zbmxz.Zxjsh = lineArray[1];

                string cmd = zbmxz.ToInsertString();
                if (ExecuteUpdateCmd(cmd))
                {
                    Console.WriteLine("插入zbmxz成功");
                    zt1 = true;
                }
                else
                {
                    Console.WriteLine("插入zbmxz失败");
                    zt1 = false;
                }
            }

            //账表分户账
            ZbfhzEntity zbfhz = new ZbfhzEntity();
            zbfhz.Yhzh = zbmxz.Zh;
            zbfhz.Ye = "20000";
            zbfhz.Bs = zbmxz.Bc;
            zbfhz.Sbrq = DateTime.Now.ToShortDateString();
            zbfhz.Hm = lineArray[6];

            int num = this.GetCountByZh(zbfhz);

            if (num == -1)
            {
                Console.WriteLine("查询账号{0}是否存在时出错", zbfhz.Yhzh);
                return false;
            }
            if (num == 0)
            {
                if (this.ExecuteUpdateCmd(zbfhz.ToInsertString()))
                {
                    Console.WriteLine("插入zbfhz成功");
                    zt2 = true;
                }
                else
                {
                    Console.WriteLine("插入zbfhz失败");
                    zt2 = false;
                }
            }
            else
            {
                if (this.ExecuteUpdateCmd(zbfhz.ToUpdateString()))
                {
                    Console.WriteLine("更新zbfhz成功");
                    zt2 = true;
                }
                else
                {
                    Console.WriteLine("更新zbfhz失败");
                    zt2 = false;
                }
            }

            return (zt1 && zt2);
        }

        /// <summary>
        /// 执行更新或插入命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private bool ExecuteUpdateCmd(string cmd)
        {
            string assemblyName = "DataAccess";
            string namespaceName = "DataAccess";
            string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
            IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

            return iDB2Operation.ExecuteDB2Update(cmd);

        }

        /// <summary>
        /// 查询zbfhz中某账号的笔数
        /// </summary>
        /// <returns></returns>
        private int GetCountByZh(ZbfhzEntity zbfhz)
        {
            string assemblyName = "DataAccess";
            string namespaceName = "DataAccess";
            string className = ConfigurationManager.AppSettings["db2Operation"].Split(new char[] { '.' })[1];
            IDB2Operation iDB2Operation = BusinessFactory.CreateInstance<IDB2Operation>(assemblyName, namespaceName, className);

            return iDB2Operation.ExecuteCountQuery(zbfhz.ToCountStringByZh());
        }
    }
}
