using System;
using Czf.Repository.Contracts;
using Czf.Repository.Contracts.Enum;
using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public partial class SmoDatabaseInfoRepository
    {
        private class TableColumnInfo : ITableColumnInfo
        {
            private readonly Column _column;
            public TableColumnInfo(Column column)
            {
                _column = column;
            }
            public string Name { get => _column.Name; }
            public bool Nullable { get => _column.Nullable; }
            public bool InPrimaryKey { get => _column.InPrimaryKey; }
            public bool Computed { get => _column.Computed; }
            public ColumnDataType ColumnDataType
            {
                get
                {
                    ColumnDataType result =
                        (ColumnDataType)_column.DataType.SqlDataType;

                    if (result == ColumnDataType.None)
                    {
                        throw new Exception("unrecognized SqlDataType");
                    }
                    return result;
                }
            }
        }
    }

    
}
