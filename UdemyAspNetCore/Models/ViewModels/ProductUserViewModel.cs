namespace UdemyAspNetCore.Models.ViewModels
{
    public class ProductUserViewModel
    {
        public ProductUserViewModel()
        {
            ProductList = new List<Product>();
        }

        public ApplicationUser ApplicationUser { get; set; }
        public IList<Product> ProductList { get; set; }
    }
}
