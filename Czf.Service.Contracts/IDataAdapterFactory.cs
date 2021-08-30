using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Service.Contracts
{
    public interface IDataAdapterFactory
    {
        IDataAdapter CreateIDataAdapter(string selectCommandText, SqlConnection sqlConnection);
    }
}
