using System;
using System.Collections.Generic;
using System.Text;
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
    class BankAsTcpClient
    {
        private IPAddress ipAddress;
        private readonly int port;
        private TcpClient myClient;
        private BinaryWriter bw;
        private BinaryReader br;
        private NetworkStream networkStream;
        /// <summary>
        /// 构造函数，完成部分初始化;
        /// </summary>
        public BankAsTcpClient()
        {
            bool isSuccess = Int32.TryParse(ConfigurationManager.AppSettings["portToGjj"], out port);
            if (!isSuccess)
            {
                Console.WriteLine("获取连接端口失败");
                LogHelper.WriteLogError("Bank as Client:", "获取连接端口失败");
            }

            ipAddress = IPAddress.Parse(ConfigurationManager.AppSettings["ipAddress"]);

        }

        public void ConnectToGjj(object obj)
        {
            myClient = new TcpClient();
            try
            {
                myClient.Connect(ipAddress, port);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("连接到公积金中心服务端失败，错误信息：{0}", ex.Message);
            }

            networkStream = myClient.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);

            Thread myThread = new Thread(new ParameterizedThreadStart(CommunicateWithGjj));
            myThread.Start(obj);
            

        }

        /// <summary>
        /// 与银行端通信;
        /// </summary>
        private void CommunicateWithGjj(object message)
        {
            SendToGjj(message as byte[]);
            ReceiveFromGjj();

        }

        /// <summary>
        /// 向GJJ发送信息;
        /// </summary>
        private void SendToGjj(byte[] msg)
        {
            try
            {
                Console.WriteLine("向公积金中心发送信息：{0},字节数：{1}", Encoding.UTF8.GetString(msg),msg.Length);
                bw.Write(msg);
                bw.Flush();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("向公积金中心发送信息出错：{0}", ex.Message);
            }
        }

        /// <summary>
        /// 从GJJ端接收信息;
        /// </summary>
        private void ReceiveFromGjj()
        {
            string recvString = null;
            try
            {
                //读取4个字节，用来确定要读取的字节数
                int countRead;
                countRead = Convert.ToInt32(Encoding.UTF8.GetString(br.ReadBytes(4)));

                byte[] recvBytes = new byte[countRead];
                recvBytes = br.ReadBytes(countRead);
                recvString = Encoding.UTF8.GetString(recvBytes);
                Console.WriteLine("从公积金中心端接收到信息:{0}", recvString);
            }
            catch
            {
                Console.WriteLine("与GJJ断开连接");
            }
        }
    }
}
