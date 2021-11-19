using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace e_Shop.ViewModel
{
    public class ChangeRoleViewModel
    {
       public string UserId { get; set; }
       public string UserEmail { get; set; }
       public List<IdentityRole> Roles { get; set; }
       public IList<string> UserRoles { get; set; }

       public ChangeRoleViewModel()
       {
           Roles = new List<IdentityRole>();
           UserRoles = new List<string>();
       }
    }
}