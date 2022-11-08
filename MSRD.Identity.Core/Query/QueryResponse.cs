using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.Core.Query
{
    public sealed class QueryResponse<T>
    {
        public IList<T> Result { get; set; }
        public int Page { get; set; }
        public int TotalPagesCount { get; set; }
        public int TotalRecordsCount { get; set; }
        public int RecordsPerPageCount { get; set; }
        public bool IsNext { get; set; }
        public bool IsPrev { get; set; }
    }
}
