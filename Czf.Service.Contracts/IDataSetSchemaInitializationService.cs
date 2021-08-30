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
        /// <summary>
        /// Returns a dataset populated with tables and columns.
        /// </summary>
        /// <param name="databaseInfo"></param>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        DataSet InitializeTableColumnsDataSet(IDatabaseInfo databaseInfo, CancellationToken stoppingToken);

        /// <summary>
        /// Will add Foreign Keys to the tables of the dataset from databaseInfo.
        /// </summary>
        /// <param name="databaseInfo">database info to get foreign keys from</param>
        /// <param name="dataSet">dataset to add the foreign keys to</param>
        /// <param name="duplicateConstraints">Constraints that can't be added because they already exist with another name</param>
        /// <param name="nullableTargetColumnForeignKeyConstraints">Constraints that are referencing a nullable column</param>
        /// <param name="sameTableTargetColumnForeignKeyConstraints">Constraints where the target and primary key are on the same table.  For instance with hierarchy relationships </param>
        /// <param name="stoppingToken"></param>
        void InitializeTableForeignKeysForAcyclicGraph(IDatabaseInfo databaseInfo,
            DataSet dataSet,
            out List<(string ConstraintName1, string ConstraintName2)> duplicateConstraints,
            out List<IForeignKeyInfo> nullableTargetColumnForeignKeyConstraints,
            out List<IForeignKeyInfo> sameTableTargetColumnForeignKeyConstraints,
            CancellationToken stoppingToken);
    }
}
