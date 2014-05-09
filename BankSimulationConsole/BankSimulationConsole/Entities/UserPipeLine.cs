using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Entities
{
    public class UserPipeLine
    {
        public BinaryReader br { get; set; }
        public BinaryWriter bw { get; set; }
        public TcpClient client { get; set; }
        private NetworkStream networkStream;


        public UserPipeLine(TcpClient client)
        {
            this.client = client;
            networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);
        }

        public void Close()
        {
            br.Close();
            bw.Close();
            client.Close();
        }
    }
}
