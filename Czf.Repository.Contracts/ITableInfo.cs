namespace Czf.Repository.Contracts
{
    public interface ITableInfo
    {
        IDatabaseInfo Parent { get; }
        string Name { get; }
        string Schema { get; }
        public ITableColumnInfoCollection Columns { get; }
        public IForeignKeyInfoCollection ForeignKeys { get; }
    }
}