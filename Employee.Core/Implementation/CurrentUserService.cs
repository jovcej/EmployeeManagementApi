using Employee.Core.Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.Implementation
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get 
            {
                var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(string.IsNullOrEmpty(id))
                {
                    throw new UnauthorizedAccessException("User id not found");
                }

                return int.Parse(id);
            }
        }

        public string UserName
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? "";
            }
        }


        public string Role
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? "";
            }
        }
    }
}
