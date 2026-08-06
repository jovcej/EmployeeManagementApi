using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain.Common
{
    public class PagedResult<T>
    {
        public required List<T> Data { get; init; }
        public int TotalCount { get; init; }
    }
}
