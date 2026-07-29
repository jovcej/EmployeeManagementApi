using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.DTO
{
    public class ImportResponseDto
    {
        public string Message { get; set; } = "";
        public int ImportedCount { get; set; }
    }
}
