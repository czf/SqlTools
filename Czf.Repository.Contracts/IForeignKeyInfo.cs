using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Repository.Contracts
{
    public interface IForeignKeyInfo
    {
        ITableInfo Parent { get; }
        string Name { get; }
        string ReferencedKey { get;}        
        string ReferencedTable { get;}
        string ReferencedTableSchema { get; }
        IForeignKeyColumnInfoCollection Columns { get; }
    
    }
}
