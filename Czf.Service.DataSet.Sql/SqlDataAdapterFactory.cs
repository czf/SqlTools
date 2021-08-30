using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Service.Contracts;

namespace Czf.Service.DataSetOperations.Sql
{
    public class SqlDataAdapterFactory : IDataAdapterFactory
    {
        public IDataAdapter CreateIDataAdapter(string selectCommandText, SqlConnection sqlConnection)
        => new SqlDataAdapter(selectCommandText, sqlConnection);
    }
}
