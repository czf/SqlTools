using System;
using System.Collections;
using Czf.Repository.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public class SmoDatabaseInfoRepository : IDatabaseInfoRepository
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
        private class TableInfoCollection : ITableInfoCollection
        {
            private readonly TableCollection _tableCollection;

            public TableInfoCollection(TableCollection tableCollection)
            {
                _tableCollection = tableCollection;
            }
            public ITableInfo this[int index] => new TableInfo( _tableCollection[index]);

            public ITableInfo this[string name] => new TableInfo(_tableCollection[name]);

            public ITableInfo this[string name, string schema] => new TableInfo(_tableCollection[name, schema]);

            public int Count => _tableCollection.Count;

            public bool IsSynchronized => _tableCollection.IsSynchronized;

            public object SyncRoot => _tableCollection.SyncRoot;

            
            void ICollection.CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
        private class TableInfo : ITableInfo
        {
            private readonly Table _table;
            public TableInfo(Table table)
            {
                _table = table;
            }
        }
    }

    
}
