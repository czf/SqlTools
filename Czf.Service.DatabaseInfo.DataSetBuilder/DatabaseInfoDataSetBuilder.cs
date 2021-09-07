using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using Czf.Repository.Contracts;
using Czf.Service.Contracts;

namespace Czf.Service.DatabaseInfo.DataSetBuilder
{
    public class DatabaseInfoDataSetBuilder : IDatabaseInfoDatasetBuilder
    {
        private readonly IDataSetSchemaInitializationService _dataSetSchemaInitializationService;

        public DatabaseInfoDataSetBuilder(IDataSetSchemaInitializationService dataSetSchemaInitializationService)
        {
            _dataSetSchemaInitializationService = dataSetSchemaInitializationService;
        }
        public DataSet CreateDataSet(IDatabaseInfo databaseInfo, CancellationToken stoppingToken)
        {
            DataSet result = _dataSetSchemaInitializationService.InitializeTableColumnsDataSet(databaseInfo, stoppingToken);
            List<IForeignKeyInfo> foreignKeys = new List<IForeignKeyInfo>();
            List<string> tableNames = new List<string>(databaseInfo.Tables.Count);
            StringBuilder stringBuilder = new StringBuilder();

            for (int a = 0; a < databaseInfo.Tables.Count && !stoppingToken.IsCancellationRequested; a++)
            {
                ITableInfo table = databaseInfo.Tables[a];
                stringBuilder.AppendLine($"Select top(0) * from [{table.Name}]");
                tableNames.Add(table.Name);
                for (int b = 0; b < table.ForeignKeys.Count; b++)
                {
                    IForeignKeyInfo foreignKey = table.ForeignKeys[b];
                    foreignKeys.Add(foreignKey);

                }

            }

            _dataSetSchemaInitializationService.InitializeTableForeignKeysForAcyclicGraph(databaseInfo, result,
                out _, out _, out _, stoppingToken);
            return result;
        }
    }
}
