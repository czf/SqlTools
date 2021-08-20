using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Repository.Contracts
{
    public interface IForeignKeyInfoCollection : ICollection, IEnumerable
    {
        IForeignKeyInfo this[int index] { get; }
        IForeignKeyInfo this[string name] { get; }
        ITableInfo Parent { get; }

    }
}
