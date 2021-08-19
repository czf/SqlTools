using System;
using System.Collections;
using Czf.Repository.Contracts;
using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public partial class SmoDatabaseInfoRepository
    {
        private class ForeignKeyColumnInfoCollection : IForeignKeyColumnInfoCollection
        {
            private readonly ForeignKeyColumnCollection _foreignKeyColumnCollection;
            public ForeignKeyColumnInfoCollection(ForeignKeyColumnCollection foreignKeyColumnCollection)
            {
                _foreignKeyColumnCollection = foreignKeyColumnCollection;
            }
            public IForeignKeyColumnInfo this[int index] => new ForeignKeyColumnInfo(_foreignKeyColumnCollection[index]);

            public IForeignKeyColumnInfo this[string name] => new ForeignKeyColumnInfo(_foreignKeyColumnCollection[name]);

            public int Count => _foreignKeyColumnCollection.Count;

            public bool IsSynchronized => _foreignKeyColumnCollection.IsSynchronized;

            public object SyncRoot => _foreignKeyColumnCollection.SyncRoot;

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
