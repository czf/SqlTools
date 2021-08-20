using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Repository.Contracts
{
    public interface IForeignKeyColumnInfo
    {
        IForeignKeyInfo Parent { get; }
        string Name { get; }
        string ReferencedColumn { get; }
    }
}
