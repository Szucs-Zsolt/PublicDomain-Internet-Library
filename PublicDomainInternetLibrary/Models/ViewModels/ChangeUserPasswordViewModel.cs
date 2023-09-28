using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

#nullable disable
namespace PublicDomainInternetLibrary.Models.ViewModels
{
    public class ChangeUserPasswordViewModel
    {
        [Required(ErrorMessage = "{0} megadása szükséges.")]
        [DataType(DataType.Password)]
        [Display(Name = "Új jelszó")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "{0} megadása szükséges.")]
        [DataType(DataType.Password)]
        [Display(Name = "Új jelszó ismét")]
        [Compare("NewPassword", ErrorMessage =
            "A két jelszó nem egyezik meg.")]
        public string ConfirmPassword { get; set; }

        [ValidateNever]
        [Display(Name = "Email cím")]
        public string UserEmail { get; set; }
    }
}
