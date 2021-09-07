using System;
using System.Collections.Generic;
using System.Data;
using Czf.Service.Contracts;
using QuikGraph.Algorithms;
using QuikGraph.Data;

namespace Czf.Service.DependencySortService
{
    public class QuikGraphDependencySortService : IDependencySortService
    {

        public IEnumerable<DataTable> SortDataTables(DataSet dataSet)
            => dataSet.ToGraph().TopologicalSort();
    }
}
