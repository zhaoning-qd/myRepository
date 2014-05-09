using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBusiness
{
    /// <summary>
    /// 一种业务的基类。该业务是Gjj发起请求，银行首先返回响应报文，然后再处理业务，如果需要则将处理结果主动发送到gjj
    /// </summary>
    public abstract class MsgFirstBusinessSuper
    {
        public abstract byte[] GenerateResponseRealTimeMsg(byte[] recvBytes);
        public abstract bool HandleBusiness(byte[] recvBytes, string whichBank);
    }
}
