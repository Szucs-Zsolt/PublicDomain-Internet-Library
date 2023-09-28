#nullable disable 
using System.ComponentModel.DataAnnotations;

namespace PublicDomainInternetLibrary.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} megadása szükséges.")]
        [Display(Name ="Szerző")]
        public string Author { get; set; }

        [Required(ErrorMessage = "{0} megadása szükséges.")]
        [Display(Name = "Könyv címe")]
        public string Title { get; set; }

        public string DownloadLink { get; set; }
    }
}