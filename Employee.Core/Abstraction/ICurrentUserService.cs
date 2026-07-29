using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.Abstraction
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string UserName { get; }
        string Role { get; }
    }
}
