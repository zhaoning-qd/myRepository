using CommonTools;
using IBusiness;
using System.IO;
using System.Threading;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BankSimulationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //银行作为服务端，监听公积金中心的请求;
            BankAsTcpServer bankServer = new BankAsTcpServer();
            Thread t = new Thread(bankServer.StartListening);
            t.Start();

            Thread.Sleep(100);
            Console.WriteLine(">>要向GJJ发送主动请求，请按A键...");
            Console.WriteLine("-----------------------------------------------------------------");
            //银行作为客户端向GJJ发送请求
            while (true)
            {

                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.A)
                {
                    Console.WriteLine("\n请选择要发送请求的业务：");
                    Console.WriteLine("1>商业贷款明细发送");
                    Console.WriteLine("2>CA认证");
                    string s = Console.ReadLine();

                    string assemblyName = "Business";
                    string namespaceName = "Business";
                    string className = string.Empty;
                    if (s == "1")
                    {
                        className = ConfigurationManager.AppSettings["LoanDetail"].Split(new char[] { '.' })[1];
                    }
                    if (s == "2")
                    {
                        className = ConfigurationManager.AppSettings["CA"].Split(new char[] { '.' })[1];
                    }

                    BusinessLaunchedByBankSuper b = BusinessFactory.CreateInstance<BusinessLaunchedByBankSuper>(assemblyName, namespaceName, className);
                    byte[] result = b.HandleBusiness();
                    //发起请求
                    LaunchRequestToGjj(Encoding.Default.GetString(result));
                }

            }
        }

        /// <summary>
        /// 向Gjj发起主动请求;
        /// </summary>
        private static void LaunchRequestToGjj(object result)//注意参数类型必须为object;
        {
            Console.WriteLine("\n\n向GJJ发起主动请求\n");
            Thread thread = new Thread(new ParameterizedThreadStart(new BankAsTcpClient().ConnectToGjj));
            thread.IsBackground = true;
            thread.Start(result);

        }
    }
}
