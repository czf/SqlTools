using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Repository.Contracts.Enum;

namespace Czf.Repository.Contracts
{
    public interface ITableColumnInfo
    {
        string Name { get; }
        bool Nullable { get; }
        bool InPrimaryKey { get; }
        bool Computed { get; }
        ColumnDataType ColumnDataType { get; }
    }
}
