using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        [Test]
        public void InitializeTableForeignKeysForAcyclicGraph_TestWithTwoTables2()
        {
            //Arrange 
            DataSet dataSet = new DataSet();


            var primaryTable = CreateSubstituteTableInfo("pk");
            var foreignTable = CreateSubstituteTableInfo("fk");


            var foreignKeyColumnInfo1 = CreateForeignKeyColumnInfo("fkcolumnInfoName", "primaryTableId");
            var foreignKeyColumnInfo2 = CreateForeignKeyColumnInfo("fkcolumnInfoName", "primaryTableId");
            var foreignKeyColumnInfoCollection1 = CreateForeignKeyColumnInfoCollection(foreignKeyColumnInfo1);
            var foreignKeyColumnInfoCollection2 = CreateForeignKeyColumnInfoCollection(foreignKeyColumnInfo2);

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

            foreignTable.ForeignKeys.Count.Returns(2);
            var foreignKey = CreateSubstituteForeignKeyInfo("FKTest", "primaryTableId", "pk", foreignTable, foreignKeyColumnInfoCollection1);
            var foreignKey2 = CreateSubstituteForeignKeyInfo("FKTest2", "primaryTableId", "pk", foreignTable, foreignKeyColumnInfoCollection2);
            foreignTable.ForeignKeys[0].Returns(foreignKey);
            foreignTable.ForeignKeys[1].Returns(foreignKey2);

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
            
            
            CollectionAssert.IsNotEmpty(duplicateConstraints);
            Assert.That(duplicateConstraints, 
                Has.One.Matches<(string ConstraintName1, string ConstraintName2)>(
                    x=> x.ConstraintName1== "Constraint matches constraint named FKTest already in collection." &&
                    x.ConstraintName2 == "FKTest2"));
        }


        [Test]
        public void InitializeTableForeignKeysForAcyclicGraph_TestWithTwoTables3()
        {
            //Arrange 
            DataSet dataSet = new DataSet();


            var primaryTable = CreateSubstituteTableInfo("pk");
            var foreignTable = CreateSubstituteTableInfo("fk");


            var foreignKeyColumnInfo1 = CreateForeignKeyColumnInfo("fkcolumnInfoName", "primaryTableId");
            var foreignKeyColumnInfoCollection = CreateForeignKeyColumnInfoCollection(foreignKeyColumnInfo1);
            
            var foreignKeyColumnInfo2 = CreateForeignKeyColumnInfo("fkcolumnInfoName2", "fkcolumnUnique");
            var foreignKeyColumnInfoCollection2 = CreateForeignKeyColumnInfoCollection(foreignKeyColumnInfo2);


            _databaseInfo.Tables.Count.Returns(2);
            _databaseInfo.Tables[0].Returns(primaryTable);
            _databaseInfo.Tables[1].Returns(foreignTable);

            var pkTable = new DataTable("pk");
            pkTable.Columns.Add(new DataColumn("primaryTableId"));
            pkTable.Columns["primaryTableId"].AllowDBNull = false;
            var fkTable = new DataTable("fk");
            fkTable.Columns.Add(new DataColumn("fkcolumnInfoName"));
            fkTable.Columns.Add(new DataColumn("fkcolumnInfoName2"));
            fkTable.Columns.Add(new DataColumn("fkcolumnUnique"));
            fkTable.Columns["fkcolumnInfoName"].AllowDBNull = false;
            fkTable.Columns["fkcolumnInfoName2"].AllowDBNull = true;
            fkTable.Columns["fkcolumnUnique"].AllowDBNull = false;
            dataSet.Tables.Add(pkTable);
            dataSet.Tables.Add(fkTable);

            foreignTable.ForeignKeys.Count.Returns(2);
            var foreignKey = CreateSubstituteForeignKeyInfo("FKTest", "primaryTableId", "pk", foreignTable, foreignKeyColumnInfoCollection);
            var foreignKey2 = CreateSubstituteForeignKeyInfo("FKTest2", "fkcolumnUnique", "fk", foreignTable, foreignKeyColumnInfoCollection2);
            foreignTable.ForeignKeys[0].Returns(foreignKey);
            foreignTable.ForeignKeys[1].Returns(foreignKey2);

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


            Assert.That(nullableTargetColumnForeignKeyConstraints,
                Has.One.Matches<IForeignKeyInfo>(
                    x => x.Name == "FKTest2" &&
                    x.ReferencedKey == "fkcolumnUnique" &&
                    x.ReferencedTable == "fk" &&
                    x.ReferencedTableSchema == "dbo"));

            CollectionAssert.IsNotEmpty(nullableTargetColumnForeignKeyConstraints.First().Columns);
            Assert.That(nullableTargetColumnForeignKeyConstraints.First().Columns,
                Has.One.Matches<IForeignKeyColumnInfo>(
                    x => x.Name == "fkcolumnInfoName2" && x.ReferencedColumn == "fkcolumnUnique"));
            
            CollectionAssert.IsEmpty(sameTableTargetColumnForeignKeyConstraints);
            CollectionAssert.IsEmpty(duplicateConstraints);
        }


        [Test]
        public void InitializeTableForeignKeysForAcyclicGraph_TestWithTwoTables4()
        {
            //Arrange 
            DataSet dataSet = new DataSet();


            var primaryTable = CreateSubstituteTableInfo("pk");
            var foreignTable = CreateSubstituteTableInfo("fk");


            var foreignKeyColumnInfo1 = CreateForeignKeyColumnInfo("fkcolumnInfoName2", "fkcolumnInfoName");
            var foreignKeyColumnInfoCollection = CreateForeignKeyColumnInfoCollection(foreignKeyColumnInfo1);

            _databaseInfo.Tables.Count.Returns(2);
            _databaseInfo.Tables[0].Returns(primaryTable);
            _databaseInfo.Tables[1].Returns(foreignTable);

            var pkTable = new DataTable("pk");
            pkTable.Columns.Add(new DataColumn("primaryTableId"));
            pkTable.Columns["primaryTableId"].AllowDBNull = false;
            var fkTable = new DataTable("fk");
            fkTable.Columns.Add(new DataColumn("fkcolumnInfoName"));
            fkTable.Columns.Add(new DataColumn("fkcolumnInfoName2"));
            fkTable.Columns["fkcolumnInfoName"].AllowDBNull = false;
            fkTable.Columns["fkcolumnInfoName2"].AllowDBNull = true;
            dataSet.Tables.Add(pkTable);
            dataSet.Tables.Add(fkTable);

            foreignTable.ForeignKeys.Count.Returns(1);
            var foreignKey = CreateSubstituteForeignKeyInfo("FKTest", "fkcolumnInfoName", "fk", foreignTable, foreignKeyColumnInfoCollection);
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
            ConstraintCollection foreignKeyConstraintCollection = dataSet.Tables["fk"]?.Constraints;
            Assert.NotNull(foreignKeyConstraintCollection);
            CollectionAssert.IsEmpty(foreignKeyConstraintCollection);

            

            CollectionAssert.IsEmpty(nullableTargetColumnForeignKeyConstraints);
            Assert.That(sameTableTargetColumnForeignKeyConstraints, Has.One.Matches<IForeignKeyInfo>(x=>x.Name == "FKTest" &&
                    x.ReferencedKey == "fkcolumnInfoName" &&
                    x.ReferencedTable == "fk" &&
                    x.ReferencedTableSchema == "dbo"));

            CollectionAssert.IsNotEmpty(sameTableTargetColumnForeignKeyConstraints.First().Columns);
            Assert.That(sameTableTargetColumnForeignKeyConstraints.First().Columns,
                Has.One.Matches<IForeignKeyColumnInfo>(
                    x => x.Name == "fkcolumnInfoName2" && x.ReferencedColumn == "fkcolumnInfoName"));

            CollectionAssert.IsEmpty(duplicateConstraints);
        }

        private ITableInfo CreateSubstituteTableInfo(string tableName, string schema ="dbo")
        {
            ITableInfo result = Substitute.For<ITableInfo>();
            result.Name.Returns(x =>tableName );
            result.Schema.Returns(x => schema);
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
            result.GetEnumerator().Returns(columns.GetEnumerator());
            return result;
        }
    }
}