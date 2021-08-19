using System;
using Czf.Repository.Contracts;
using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public partial class SmoDatabaseInfoRepository
    {
        private class ForeignKeyInfo : IForeignKeyInfo
        {
            private readonly ForeignKey _foreignKey;
            public ForeignKeyInfo(ForeignKey foreignKey)
            {
                _foreignKey = foreignKey;
            }

            public string ReferencedKey => _foreignKey.ReferencedKey;

            public string ReferencedTable => _foreignKey.ReferencedTable;

            public string ReferencedTableSchema => _foreignKey.ReferencedTableSchema;

            public IForeignKeyColumnInfoCollection Columns => throw new NotImplementedException();
        }
    }

    
}
