using System;
using System.Threading;
using Czf.Repository.Contracts;
using Czf.Service.Contracts;
using System.Data;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using QuikGraph.Data;
using QuikGraph.Algorithms;
using System.Diagnostics;

namespace Czf.Service.DataSetOperations.Sql
{
    public class SqlDataSetSchemaInitializationService : IDataSetSchemaInitializationService
    {
        private readonly IDataAdapterFactory _dataAdapterFactory;
        private readonly SqlConnection _sqlConnection;
        private readonly bool _exclude;

        public SqlDataSetSchemaInitializationService(IOptions<SqlDataSetSchemaInitializationServiceOptions> options, IDataAdapterFactory dataAdapterFactory)
        {
            string connectionString = options.Value.ConnectionString;
            _sqlConnection = new SqlConnection(connectionString);
            _exclude = options.Value.Exclude;
            _dataAdapterFactory = dataAdapterFactory;
        }
        public DataSet InitializeTableColumnsDataSet(IDatabaseInfo databaseInfo, CancellationToken stoppingToken)
        {
            DataSet result = new DataSet();
            StringBuilder stringBuilder = new StringBuilder();
            
            List<string> tableNames = new List<string>(databaseInfo.Tables.Count);
            for (int a = 0; a < databaseInfo.Tables.Count && !stoppingToken.IsCancellationRequested; a++)
            {
                ITableInfo table = databaseInfo.Tables[a];
                stringBuilder.AppendLine($"Select top(0) * from [{table.Name}]");
                tableNames.Add(table.Name);
                
            }
            IDataAdapter dataAdapter = _dataAdapterFactory.CreateIDataAdapter(stringBuilder.ToString(), _sqlConnection); //new SqlDataAdapter(stringBuilder.ToString(), _sqlConnection);
            dataAdapter.FillSchema(result, SchemaType.Source);
            dataAdapter.Fill(result);

            for (int a = 0; a < tableNames.Count && !stoppingToken.IsCancellationRequested; a++)
            {
                result.Tables[a].TableName = tableNames[a];
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseInfo"></param>
        /// <param name="dataSet"></param>
        /// <param name="duplicateConstraints"></param>
        /// <param name="nullableTargetColumnForeignKeyConstraints"></param>
        /// <param name="sameTableTargetColumnForeignKeyConstraints"></param>
        /// <param name="stoppingToken"></param>
        public void InitializeTableForeignKeysForAcyclicGraph(
            IDatabaseInfo databaseInfo, 
            DataSet dataSet, 
            out List<(string ConstraintName1, string ConstraintName2)> duplicateConstraints,
            out List<IForeignKeyInfo> nullableTargetColumnForeignKeyConstraints,
            out List<IForeignKeyInfo> sameTableTargetColumnForeignKeyConstraints,
            CancellationToken stoppingToken)
        {
            bool isAcyclic = true;
            List<IForeignKeyInfo> foreignKeys = new List<IForeignKeyInfo>();

            for (int a = 0; a < databaseInfo.Tables.Count && !stoppingToken.IsCancellationRequested; a++)
            {
                ITableInfo table = databaseInfo.Tables[a];
                for (int b = 0; b < table.ForeignKeys.Count; b++)
                {
                    IForeignKeyInfo foreignKey = table.ForeignKeys[b];
                    foreignKeys.Add(foreignKey);

                }
            }

            List<string> sameColumnForeignKeyNames = new List<string>();
            nullableTargetColumnForeignKeyConstraints = new List<IForeignKeyInfo>();
            sameTableTargetColumnForeignKeyConstraints = new List<IForeignKeyInfo>();
            duplicateConstraints = new List<(string ConstraintName1, string ConstraintName2)>();

            foreach (var fk in foreignKeys)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                List<DataColumn> columns = new List<DataColumn>();
                List<DataColumn> referencedColumns = new List<DataColumn>();
                bool isSameColumn = false;
                bool isSameTable = false;
                bool isNullableTargetColumn = false;
                for (int c = 0; c < fk.Columns.Count && !stoppingToken.IsCancellationRequested; c++)
                {
                    IForeignKeyColumnInfo foreignKeyColumn = fk.Columns[c];
                    if (fk.Parent.Name == fk.ReferencedTable && fk.Parent.Schema == fk.ReferencedTableSchema && foreignKeyColumn.Name == foreignKeyColumn.ReferencedColumn)
                    {
                        isSameColumn = true;
                        sameColumnForeignKeyNames.Add(fk.Name);
                        Debug.WriteLine(fk.Name);
                        break;
                    }
                    if (fk.Parent.Name == fk.ReferencedTable && fk.Parent.Schema == fk.ReferencedTableSchema)
                    {
                        isSameTable = true;
                        sameTableTargetColumnForeignKeyConstraints.Add(fk);
                        break;
                    }
                    if (dataSet.Tables[fk.Parent.Name].Columns[foreignKeyColumn.Name].AllowDBNull)
                    {
                        isNullableTargetColumn = true;
                        nullableTargetColumnForeignKeyConstraints.Add(fk);
                        break;
                    }
                    if (_exclude && !dataSet.Tables.Contains(fk.ReferencedTable))
                    {
                        continue;
                    }
                    referencedColumns.Add(dataSet.Tables[fk.Parent.Name].Columns[foreignKeyColumn.Name]);
                    columns.Add(dataSet.Tables[fk.ReferencedTable].Columns[foreignKeyColumn.ReferencedColumn]);

                }
                if (!isNullableTargetColumn && !isSameColumn && !isSameTable && columns.Count > 0 && referencedColumns.Count > 0)
                {
                    ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(fk.Name, columns.ToArray(), referencedColumns.ToArray());
                    try
                    {
                        //DataRelation dataRelation = new DataRelation(fk.Name, referencedColumns.ToArray(), columns.ToArray());
                        dataSet.Tables[fk.Parent.Name].Constraints.Add(foreignKeyConstraint);
                        dataSet.Tables[fk.Parent.Name].ParentRelations.Add(fk.Name, columns.ToArray(), referencedColumns.ToArray(), false);
                    }
                    catch (DataException dataException) when (dataException.Message.Contains("Constraint matches constraint named"))
                    {
                        duplicateConstraints.Add((dataException.Message, foreignKeyConstraint.ConstraintName));
                        Console.WriteLine(dataException.Message + " " + foreignKeyConstraint.ConstraintName);
                    }
                }
                if (!isSameColumn && !isSameTable && isAcyclic && !dataSet.ToGraph().IsDirectedAcyclicGraph())
                {
                    Console.WriteLine("table is no longer acyclic graph after adding: " + fk.Name);
                    isAcyclic = false;
                }



            }

        }

        public class SqlDataSetSchemaInitializationServiceOptions
        {
            public string ConnectionString { get; set; }
            public bool Exclude { get; set; }
        }

    }
}
