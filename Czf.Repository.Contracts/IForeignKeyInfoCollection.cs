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
        public IForeignKeyInfo this[int index] { get; }
        public IForeignKeyInfo this[string name] { get; }

    }
}
