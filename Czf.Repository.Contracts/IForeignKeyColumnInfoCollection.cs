using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Repository.Contracts
{
    public interface IForeignKeyColumnInfoCollection : ICollection, IEnumerable
    {
        IForeignKeyColumnInfo this[int index] { get; }
        IForeignKeyColumnInfo this[string name] { get; }
    }
}
