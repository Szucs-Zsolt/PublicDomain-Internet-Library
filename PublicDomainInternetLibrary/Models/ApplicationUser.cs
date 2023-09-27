using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable
namespace PublicDomainInternetLibrary.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string Name { get; set; }

        [NotMapped]
        public string RoleName { get; set; }
    }
}
