using Microsoft.AspNetCore.Identity;
using System;

namespace SocialNetwork.Domain.Entities
{
    public class User : IdentityUser
    {
        public string UriImageProfile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
