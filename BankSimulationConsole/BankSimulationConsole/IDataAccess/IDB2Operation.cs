using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IDataAccess
{
    /// <summary>
    /// 所有操作DB2数据库的基类
    /// </summary>
    public interface IDB2Operation
    {
        /// <summary>
        /// ADO.NET 更新DB2数据库中的表;
        /// </summary>
        bool ExecuteDB2Update(string command);

        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        int ExecuteCountQuery(string command);
    }
}
