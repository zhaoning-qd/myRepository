using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities.BllModels
{
    /// <summary>
    /// 网厅网银转账对账交易实体
    /// </summary>
    public class WtWyzhDzModel
    {
        /// <summary>
        /// 交易码
        /// </summary>
        public string Jym { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string Yhzh { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public string Qsrq { get; set; }

        /// <summary>
        /// 终止日期
        /// </summary>
        public string Zzrq { get; set; }

        /// <summary>
        /// 总笔数
        /// </summary>
        public string Zbs { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public string Zje { get; set; }
    }
}
