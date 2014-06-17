using System;
using System.Collections.Generic;

namespace Entities.BllModels
{
    /// <summary>
    /// 网厅支取--实时交易日终对账实体
    /// </summary>
    public class WtzqSsjyModel
    {
        /// <summary>
        /// 交易码
        /// </summary>
        public string Jym { get; set; }

        /// <summary>
        /// 开始批次号
        /// </summary>
        public string Kspch { get; set; }

        /// <summary>
        /// 结束批次号
        /// </summary>
        public string Jspch { get; set; }

        /// <summary>
        /// 机构码
        /// </summary>
        public string Jgm { get; set; }

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
