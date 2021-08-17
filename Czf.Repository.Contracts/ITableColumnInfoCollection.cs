using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Repository.Contracts
{
    public interface ITableColumnInfoCollection : ICollection, IEnumerable
    {
        ITableColumnInfo this[int index] { get; }
        ITableColumnInfo this[string name] { get; }
    }
}
