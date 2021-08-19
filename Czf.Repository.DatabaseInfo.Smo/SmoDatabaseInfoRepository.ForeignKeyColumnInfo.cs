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
        }
    }

    
}
