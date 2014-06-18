using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities.BllModels
{
    /// <summary>
    /// 贷款发放业务实体
    /// </summary>
    public class DkfyModel
    {
        /// <summary>
        /// 交易码
        /// </summary>
        public string Jym { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string Pch { get; set; }

        /// <summary>
        /// 付款人账号(中心)
        /// </summary>
        public string Fkrzh { get; set; }

        /// <summary>
        /// 付款人名称(中心)
        /// </summary>
        public string Fkrmc { get; set; }

        /// <summary>
        /// 收款人账号
        /// </summary>
        public string Skrzh { get; set; }

        /// <summary>
        /// 收款人名称
        /// </summary>
        public string Skrmc { get; set; }

        /// <summary>
        /// 收款银行名称
        /// </summary>
        public string Skyhmc { get; set; }

        /// <summary>
        /// 收款银行机构码
        /// </summary>
        public string Skyhjgm { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string Je { get; set; }
    }
}
