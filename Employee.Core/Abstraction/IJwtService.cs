using Employee.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.Abstraction
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
