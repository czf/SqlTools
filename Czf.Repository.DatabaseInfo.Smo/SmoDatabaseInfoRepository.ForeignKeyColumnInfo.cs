using Czf.Repository.Contracts;
using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public partial class SmoDatabaseInfoRepository
    {
        private class ForeignKeyColumnInfo : IForeignKeyColumnInfo 
        {
            private readonly ForeignKeyColumn _foreignKeyColumn;

            public ForeignKeyColumnInfo(ForeignKeyColumn foreignKeyColumn)
            {
                _foreignKeyColumn = foreignKeyColumn;
            }

            public IForeignKeyInfo Parent => new ForeignKeyInfo(_foreignKeyColumn.Parent);

            public string Name => _foreignKeyColumn.Name;

            public string ReferencedColumn => _foreignKeyColumn.ReferencedColumn;
        }
    }

    
}
