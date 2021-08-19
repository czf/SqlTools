using System;
using Czf.Repository.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public partial class SmoDatabaseInfoRepository : IDatabaseInfoRepository
    {
        private readonly Server _server;

        public SmoDatabaseInfoRepository(IOptions<SmoDatabaseInfoRepositoryOptions> options)
        {
            string connectionString = options.Value.ConnectionString;
            ServerConnection serverConnection = new ServerConnection(connectionString);
            _server = new Server(serverConnection);
            _server.SetDefaultInitFields(typeof(ForeignKey), "ReferencedKey", "ReferencedTable", "ReferencedTableSchema"/*, "Columns"*/, "Name"/*, "Parent"*/);
            _server.SetDefaultInitFields(typeof(ForeignKeyColumn), "Name"/*, "Parent"*/, "ReferencedColumn");
            _server.SetDefaultInitFields(typeof(Table), /*"Columns",*/ "Name", "IsSystemObject"/*, "ForeignKeys"*/, "Schema");


        }
        public IDatabaseInfo GetDatabaseInfo(string databaseName)
        {
            Database database = _server.Databases[databaseName];
            if(database == null)
            {
                throw new Exception("No such database");
            }
            database.InitTableColumns();
            database.InitChildCollection(new Urn("Table/ForeignKey"), false);
            database.InitChildCollection(new Urn("Table/ForeignKey/Column"), false);

            return new DatabaseInfo(database);
        }
            
        
        public class SmoDatabaseInfoRepositoryOptions
        {
            public string ConnectionString{ get; set; }
        }
    }

    
}
