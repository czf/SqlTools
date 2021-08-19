using System;
using System.Collections;
using Czf.Repository.Contracts;
using Microsoft.SqlServer.Management.Smo;

namespace Czf.Repository.DatabaseInfo.Smo
{
    public partial class SmoDatabaseInfoRepository
    {
        private class TableColumnInfoCollection : ITableColumnInfoCollection
        {
            private readonly ColumnCollection _columnCollection;
            public TableColumnInfoCollection(ColumnCollection columnCollection)
            {
                _columnCollection = columnCollection;
            }
            public ITableColumnInfo this[int index] => new TableColumnInfo(_columnCollection[index]);

            public ITableColumnInfo this[string name] => new TableColumnInfo(_columnCollection[name]);

            public int Count => _columnCollection.Count;

            public bool IsSynchronized => _columnCollection.IsSynchronized;

            public object SyncRoot => _columnCollection.SyncRoot;

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
