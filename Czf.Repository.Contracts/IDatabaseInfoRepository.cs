using System;

namespace Czf.Repository.Contracts
{
    public interface IDatabaseInfoRepository
    {
        IDatabaseInfo GetDatabaseInfo(string database);
    }
}
