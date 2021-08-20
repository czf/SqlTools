using System;
using System.Collections;
using Czf.Repository.Contracts;
using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public partial class SmoDatabaseInfoRepository
    {
        private class ForeignKeyInfoCollection : IForeignKeyInfoCollection
        {
            private ForeignKeyCollection _foreignKeyCollection;
            public ForeignKeyInfoCollection(ForeignKeyCollection foreignKeyCollection)
            {
                _foreignKeyCollection = foreignKeyCollection;
            }

            public IForeignKeyInfo this[int index] => new ForeignKeyInfo(_foreignKeyCollection[index]);

            public IForeignKeyInfo this[string name] => new ForeignKeyInfo(_foreignKeyCollection[name]);

            public int Count => _foreignKeyCollection.Count;

            public bool IsSynchronized => _foreignKeyCollection.IsSynchronized;

            public object SyncRoot => _foreignKeyCollection.SyncRoot;

            public ITableInfo Parent => new TableInfo(_foreignKeyCollection.Parent);

            public void CopyTo(Array array, int index)
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
