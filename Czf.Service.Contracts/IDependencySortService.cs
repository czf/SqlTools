﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Service.Contracts
{
    public interface IDependencySortService
    {
        IEnumerable<DataTable> SortDataTables(DataSet dataSet);
    }
}
