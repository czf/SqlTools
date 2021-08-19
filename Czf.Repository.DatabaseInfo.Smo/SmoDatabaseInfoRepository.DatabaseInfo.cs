using System;
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
        private class DatabaseInfo : IDatabaseInfo
        {
            private readonly Database _database;
            public DatabaseInfo(Database database)
            {
                _database = database;
            }
            public ITableInfoCollection TableInfos
            {
                get => new TableInfoCollection(_database.Tables);
            }
        }
    }
}
