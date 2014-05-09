using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Configuration;
using System.Threading;
using Entities;
using IBusiness;
using CommonTools;

namespace BankSimulationConsole
{
    class BankAsTcpServer
    {
        private TcpListener myListener;
        private IPAddress[] localAddr;
        private IPAddress ipAddress;
        private readonly int port;

        private string whichBank;

        /// <summary>
        /// 构造函数，完成部分初始化;
        /// </summary>
        public BankAsTcpServer()
        {
            string strPort = ConfigurationManager.AppSettings["port"];
            bool isSuccess = Int32.TryParse(strPort, out port);
            if (!isSuccess)
            {
                Console.WriteLine("监听端口号设置错误，请检查");
                LogHelper.WriteLogError("Bank as server:", "监听端口号设置错误");
            }

            localAddr = Dns.GetHostAddresses(Dns.GetHostName());
           // ipAddress = localAddr[0];
            ipAddress = new IPAddress(new byte[] {127,0,0,1 });
            Console.WriteLine("\t\t\t银行模拟通讯程序");
            Console.WriteLine("-----------------------------------------------------------------");
        }


        /// <summary>
        /// 开始监听;
        /// </summary>
        public void StartListening()
        {
            TcpClient newClient = null;
            myListener = new TcpListener(ipAddress, port);
            myListener.Start(10);//最多监听10个连接请求
            Console.WriteLine(">>在端口{0}开始监听,等待公积金中心发起连接请求...", port);
            while (true)
            {
                try
                {
                    newClient = myListener.AcceptTcpClient();//接收客户端的连接请求，创建一个连接通道;
                    Console.WriteLine(">>与客户端{0}建立连接",newClient.Client.RemoteEndPoint);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("建立与客户端的连接错误");
                    LogHelper.WriteLogException("建立与客户端的连接错误",ex);
                    break;
                }

                //在新线程中打开与客户端的TCP连接通道，进行监听;
                UserPipeLine user = new UserPipeLine(newClient);
                Thread newThread = new Thread(new ParameterizedThreadStart(StartListeningClient));
                newThread.Start(user);               
            }
        }

        /// <summary>
        /// 监听客户端;
        /// </summary>
        private void StartListeningClient(object user)
        {
            UserPipeLine u = user as UserPipeLine;
            TcpClient myClient = u.client;
            while (true)
            {
                try
                {
                    string recvString = null;
                    whichBank = Encoding.Default.GetString(u.br.ReadBytes(2));//行别;
                    //读取4个字节，用来确定要读取的字节数;
                    int countRead;
                    countRead = Convert.ToInt32(Encoding.Default.GetString(u.br.ReadBytes(4)));

                    byte[] recvBytes = new byte[countRead];
                    recvBytes = u.br.ReadBytes(countRead);
                    recvString = Encoding.Default.GetString(recvBytes);

                    Console.WriteLine("接收到来自GJJ的报文：\n{0}", recvString);
                    //业务处理过程;
                    HandleGjjRequest(u, recvBytes,whichBank);
                    Console.WriteLine("-----------------------------------------------------------------");
                    break;
                }
                catch(Exception ex)
                {
                    string s = ex.Message;
                    Console.WriteLine("与客户端断开连接,原因：{0}",s);
                    break;
                }
            }
        }

        /// <summary>
        /// 向GJJ发送消息，byte[];
        /// </summary>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        private void SendToGjj(UserPipeLine user, byte[] msg)
        {
            try
            {
                Console.WriteLine("向GJJ发送信息：\n{0},字节数：{1}", Encoding.Default.GetString(msg),msg.Length);
                user.bw.Write(msg);
                user.bw.Flush();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("向GJJ发送信息时出错,错误信息：{0}", ex.Message);
                LogHelper.WriteLogException("向GJJ发送信息时发生异常", ex);
            }
        }

        /// <summary>
        /// 处理公积金中心的请求;
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private void HandleGjjRequest(UserPipeLine user,byte[] recvBytes,string whichBank)
        {
            //获取交易码，根据交易码调用具体的业务对象;
            string transcationCode;
            transcationCode = Encoding.UTF8.GetString(recvBytes).Substring(0, 4);
            byte[] returnBytes;

            string assemblyName = "Business";
            string namespaceName = "Business";
            string className = ConfigurationManager.AppSettings[transcationCode].Split(new char[]{'.'})[1];

            if (transcationCode == "2000" || transcationCode == "2006")
            {
                MsgFirstBusinessSuper m = BusinessFactory.CreateInstance<MsgFirstBusinessSuper>(assemblyName, namespaceName, className);
                SendToGjj(user, m.GenerateResponseRealTimeMsg(recvBytes));
                m.HandleBusiness(recvBytes, whichBank);
            }
            else
            {
                GjjBusinessSuper g = BusinessFactory.CreateInstance<GjjBusinessSuper>(assemblyName, namespaceName, className);
                returnBytes = g.HandleBusiness(recvBytes, whichBank);
                SendToGjj(user, returnBytes);
            }
        }

        /// <summary>
        /// 向Gjj发起主动请求;
        /// </summary>
        private void LaunchRequestToGjj(object result)//注意参数类型必须为object;
        {
            Console.WriteLine("\n\n向GJJ发起主动请求\n");
            Thread thread = new Thread(new ParameterizedThreadStart(new BankAsTcpClient().ConnectToGjj));
            thread.IsBackground = true;
            thread.Start(result);

        }
    }
}
