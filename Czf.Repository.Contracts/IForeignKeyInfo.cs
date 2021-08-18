using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Repository.Contracts
{
    public interface IForeignKeyInfo
    {
        string ReferencedKey { get;}        
        string ReferencedTable { get;}
        string ReferencedTableSchema { get; }
        public IForeignKeyColumnInfoCollection Columns { get; }
    
    }
}
