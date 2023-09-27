namespace PublicDomainInternetLibrary.Models.ViewModels
{
    public class BookIndexViewModel
    {
        public string? FindAuthor { get; set; }
        public string? FindTitle { get; set; }

        public IEnumerable<Book>? Books { get; set; }
    }
}
