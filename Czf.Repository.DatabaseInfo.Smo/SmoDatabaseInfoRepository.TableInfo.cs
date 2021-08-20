﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Repository.Contracts;

using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public partial class SmoDatabaseInfoRepository
    {
        private class TableInfo : ITableInfo
        {
            private readonly Table _table;
            public TableInfo(Table table)
            {
                _table = table;
            }
            public string Name { get => _table.Name; }
            public string Schema { get => _table.Schema; }
            public ITableColumnInfoCollection Columns{ get => new TableColumnInfoCollection(_table.Columns); }
            public IForeignKeyInfoCollection ForeignKeys { get => new ForeignKeyInfoCollection(_table.ForeignKeys); }
            public IDatabaseInfo Parent { get => new DatabaseInfo(_table.Parent); }
        }

    }
}
