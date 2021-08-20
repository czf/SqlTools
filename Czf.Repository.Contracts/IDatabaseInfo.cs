namespace Czf.Repository.Contracts
{
    public interface IDatabaseInfo 
    {
        ITableInfoCollection Tables { get; }
        string Name { get; }
    }
}