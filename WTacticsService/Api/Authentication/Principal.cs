using System;
using System.Security.Principal;
using WTacticsDAL;

namespace WTacticsService.Api.Authentication
{
    public class Principal : IPrincipal
    {
        public Principal(Guid userId, string email, string role)
        {
            UserId = userId;
            Role = role;
            Identity = new Identity { Name = email }; // Name is not allowed to be null, therefore we take email because that cannot be null
        }

        public Guid UserId { get; }
        
        public string Role { get;  }

        public IIdentity Identity { get; }

        public bool IsInRole(string role)
        {
            return Equals(Role, role);
        }

    
    }

    public class Identity : IIdentity
    {
        public string AuthenticationType { get; } = "Bearer";

        public bool IsAuthenticated { get; } = true;

        public string Name { get; set; }
    }
}