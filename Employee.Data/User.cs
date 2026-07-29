using Employee.Domain.Enums;
using Employee.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain
{
    public class User
    {
        public int Id { get; private set; }
        public Username Username { get; private set; } = null!;
        public PasswordHash PasswordHash { get; private set; } = null!;
        public UserRole Role { get; private set; }


        private User()
        {

        }

        public User(Username username,
            UserRole role)
        {
            Username = username;
            Role = role;
        }

        public void ChangeRole(UserRole role)
        { 
            Role = role;
        }

        public void ChangePassword(PasswordHash password)
        { 
            PasswordHash = password;
        }
    }
}
