using PublicDomainInternetLibrary.Models;
using PublicDomainInternetLibrary.Data;
using Microsoft.AspNetCore.Identity;

namespace PublicDomainInternetLibrary.Models.ViewModels
{
    public class UserIndexViewModel
    {
        public List<IdentityUser> LibrarianList { get; set; } = new List<IdentityUser>();
        public List<IdentityUser> UserList { get; set; } = new List<IdentityUser>();
    }
}
