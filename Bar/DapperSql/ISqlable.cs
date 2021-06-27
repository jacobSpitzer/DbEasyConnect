using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bar.DapperSql
{
    public interface ISqlable<T>
    {
        public IQueryable<T> Empty { get; set; }
        string SqlString { get; set; }
        bool IsQuery();
        int ExecuteCommand();
        List<T> ExecuteQuery();
    }
}
