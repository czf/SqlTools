using System;
using Czf.Repository.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public partial class SmoDatabaseInfoRepository : IDatabaseInfoRepository, IDisposable
    {
        private readonly Server _server;
        private readonly SqlConnection _sqlConnection;
        private bool disposedValue;

        public SmoDatabaseInfoRepository(IOptions<SmoDatabaseInfoRepositoryOptions> options)
        {
            string connectionString = options.Value.ConnectionString;
            _sqlConnection = new SqlConnection(connectionString);
            ServerConnection serverConnection = new ServerConnection(_sqlConnection);

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
            public const string SmoDatabaseInfoRepositoryOptionsKey = "SmoDatabaseInfoRepository";
            public string ConnectionString{ get; set; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _sqlConnection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SmoDatabaseInfoRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    
}
