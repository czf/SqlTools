using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Czf.Repository.Contracts;

namespace Czf.Service.Contracts
{
    public interface IDataSetSchemaInitializationService
    {
        DataSet InitializeTableColumnsDataSet(IDatabaseInfo databaseInfo, CancellationToken stoppingToken);
    }
}
