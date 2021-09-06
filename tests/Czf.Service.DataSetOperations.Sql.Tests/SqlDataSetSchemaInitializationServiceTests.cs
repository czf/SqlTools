using System.Collections.Generic;
using System.Data;
using System.Threading;
using Czf.Repository.Contracts;
using Czf.Service.Contracts;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using static Czf.Service.DataSetOperations.Sql.SqlDataSetSchemaInitializationService;

namespace Czf.Service.DataSetOperations.Sql.Tests
{
    public class SqlDataSetSchemaInitializationServiceTests
    {
        private SqlDataSetSchemaInitializationService _service;
        private IDataAdapter _dataAdapter;
        private IDataAdapterFactory _dataAdapterFactory;
        private IDatabaseInfo _databaseInfo;

        [SetUp]
        public void Setup()
        {
            SqlDataSetSchemaInitializationServiceOptions options = new SqlDataSetSchemaInitializationServiceOptions()
            {
                ConnectionString = "data source=localhost;database=temp;Integrated Security=SSPI;persist security info=True;",
                Exclude = false
            };
            _databaseInfo = Substitute.For<IDatabaseInfo>();

            IOptions<SqlDataSetSchemaInitializationServiceOptions> optionsContainer = Substitute.For<IOptions<SqlDataSetSchemaInitializationServiceOptions>>();
            optionsContainer.Value.ReturnsForAnyArgs(options);

            _dataAdapter = Substitute.For<IDataAdapter>();
            _dataAdapterFactory = Substitute.For<IDataAdapterFactory>();
            _dataAdapterFactory.CreateIDataAdapter(null, null).ReturnsForAnyArgs(_dataAdapter);
            
            _service = new(optionsContainer, _dataAdapterFactory);
        }

        [Test]
        public void InitializeTableColumnsDataSet_Returns_DataSet()
        {
            //Arrange 
            DataSet dataSet;
            string sqlText = null;
            var table = CreateSubstituteTableInfo("temp");

            _databaseInfo.Tables.Count.Returns(1);
            _databaseInfo.Tables[0].Returns(table);
            
            _dataAdapter.WhenForAnyArgs(x => x.FillSchema(null, SchemaType.Source)).Do(x =>
              {
                  dataSet = x.ArgAt<DataSet>(0);
                  dataSet.Tables.Add();
                  dataSet.Tables[0].TableName = "random";
  
              });

            _dataAdapterFactory.WhenForAnyArgs(x =>
                x.CreateIDataAdapter(null, null)).
                Do(x =>
                {
                    sqlText = x.ArgAt<string>(0);
                });
            
            //Act
            DataSet result = _service.InitializeTableColumnsDataSet(_databaseInfo, CancellationToken.None);

            //Assert
            Assert.That(sqlText, Does.Contain("Select top(0) * from [temp]"));
            Assert.That(result.Tables[0].TableName, Is.EqualTo("temp"));            
        }



        [Test]
        public void InitializeTableForeignKeysForAcyclicGraph_TestWithTwoTables()
        {
            //Arrange 
            DataSet dataSet = new DataSet();
            

            var primaryTable = CreateSubstituteTableInfo("pk");
            var foreignTable = CreateSubstituteTableInfo("fk");


            var foreignKeyColumnInfo1 = CreateForeignKeyColumnInfo("fkcolumnInfoName", "primaryTableId");
            var foreignKeyColumnInfoCollection = CreateForeignKeyColumnInfoCollection(foreignKeyColumnInfo1);

            _databaseInfo.Tables.Count.Returns(2);
            _databaseInfo.Tables[0].Returns(primaryTable);
            _databaseInfo.Tables[1].Returns(foreignTable);

            var pkTable = new DataTable("pk");
            pkTable.Columns.Add(new DataColumn("primaryTableId"));
            pkTable.Columns["primaryTableId"].AllowDBNull = false;
            var fkTable = new DataTable("fk");
            fkTable.Columns.Add(new DataColumn("fkcolumnInfoName"));
            fkTable.Columns["fkcolumnInfoName"].AllowDBNull = false;    
            dataSet.Tables.Add(pkTable);
            dataSet.Tables.Add(fkTable);

            foreignTable.ForeignKeys.Count.Returns(1);
            var foreignKey = CreateSubstituteForeignKeyInfo("FKTest","primaryTableId",  "pk", foreignTable, foreignKeyColumnInfoCollection);
            foreignTable.ForeignKeys[0].Returns(foreignKey);

            //Act
            _service.InitializeTableForeignKeysForAcyclicGraph(
                _databaseInfo,
                dataSet,
                out List<(string ConstraintName1, string ConstraintName2)> duplicateConstraints,
            out List<IForeignKeyInfo> nullableTargetColumnForeignKeyConstraints,
            out List<IForeignKeyInfo> sameTableTargetColumnForeignKeyConstraints,
            CancellationToken.None);

            //Assert
            ForeignKeyConstraint foreignKeyConstraintResult = (ForeignKeyConstraint)dataSet.Tables["fk"].Constraints[0];
            Assert.NotNull(foreignKeyConstraintResult);
            CollectionAssert.IsNotEmpty(foreignKeyConstraintResult.Columns);
            Assert.That(foreignKeyConstraintResult.Columns, Has.Length.EqualTo(1));
            Assert.AreEqual(foreignKeyConstraintResult.ConstraintName, "FKTest");

            CollectionAssert.IsEmpty(nullableTargetColumnForeignKeyConstraints);
            CollectionAssert.IsEmpty(sameTableTargetColumnForeignKeyConstraints);
            CollectionAssert.IsEmpty(duplicateConstraints);

        }







        private ITableInfo CreateSubstituteTableInfo(string tableName)
        {
            ITableInfo result = Substitute.For<ITableInfo>();
            result.Name.Returns(x =>tableName );
            return result;
        }

        private IForeignKeyColumnInfo CreateForeignKeyColumnInfo(string name, string referencedColumn)
        {
            IForeignKeyColumnInfo result = Substitute.For<IForeignKeyColumnInfo>();
            result.Name.Returns(x => name);
            result.ReferencedColumn.Returns(x => referencedColumn);
            return result;
        }

        private IForeignKeyInfo CreateSubstituteForeignKeyInfo(string fkName, 
            string referencedKey, 
            string referencedTable,
            ITableInfo parent,
            IForeignKeyColumnInfoCollection foreignKeyColumnInfoCollection,
            string referencedTableSchema = "dbo")
        {
            IForeignKeyInfo result = Substitute.For<IForeignKeyInfo>();
            result.Name.Returns(x => fkName);
            result.ReferencedKey.Returns(x => referencedKey);
            result.ReferencedTable.Returns(x => referencedTable);
            result.Parent.Returns(x => parent);
            result.ReferencedTableSchema.Returns(x => referencedTableSchema);
            result.Columns.Returns(foreignKeyColumnInfoCollection);
            return result;
        }

        private IForeignKeyColumnInfoCollection CreateForeignKeyColumnInfoCollection(params IForeignKeyColumnInfo[] columns)
        {
            IForeignKeyColumnInfoCollection result = Substitute.For<IForeignKeyColumnInfoCollection>();
            result.Count.Returns(columns.Length);
            for(int a = 0; a<columns.Length; a++)
            {
                result[a].Returns(columns[a]);
                result[columns[a].Name].Returns(columns[a]);
            }
            return result;
        }
    }
}