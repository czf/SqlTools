using System.Collections;

namespace Czf.Repository.Contracts
{
    public interface ITableInfoCollection : ICollection, IEnumerable
    {
        ITableInfo this[int index] { get; }
        ITableInfo this[string name] { get; }
        ITableInfo this[string name, string schema] { get; }        


    }
}