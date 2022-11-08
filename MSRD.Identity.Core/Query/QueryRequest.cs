using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.Core.Query
{
    public sealed class QueryRequest
    {
        public int Rows { get; set; }
        public int Offset { get; set; }
        public string? SortField { get; set; }
        public int? SortOrder { get; set; }

    }
}
