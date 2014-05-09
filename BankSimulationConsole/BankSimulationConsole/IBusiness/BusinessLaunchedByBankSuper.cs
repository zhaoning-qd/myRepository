using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBusiness
{
    /// <summary>
    /// 基类，其子类表示的业务完全由银行先发起
    /// </summary>
    public abstract class BusinessLaunchedByBankSuper
    {
        public abstract byte[] HandleBusiness();
    }
}
