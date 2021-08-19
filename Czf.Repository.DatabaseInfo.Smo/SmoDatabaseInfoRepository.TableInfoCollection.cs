using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Repository.Contracts;
using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public partial class SmoDatabaseInfoRepository
    {
        private class TableInfoCollection : ITableInfoCollection
        {
            private readonly TableCollection _tableCollection;

            public TableInfoCollection(TableCollection tableCollection)
            {
                _tableCollection = tableCollection;
            }
            public ITableInfo this[int index] => new TableInfo(_tableCollection[index]);

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
    }
}
