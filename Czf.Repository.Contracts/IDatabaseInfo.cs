namespace Czf.Repository.Contracts
{
    public interface IDatabaseInfo 
    {
        ITableInfoCollection TableInfos { get; }
    }
}