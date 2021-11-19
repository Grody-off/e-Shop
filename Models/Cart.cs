using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace e_Shop.Models
{
    public class Cart
    {
        public Cart()
        {
            Products = new List<Product>();
        }
        [Key]
        public int CartId { get; set; }
        public string UserId { get; set; }
        public ICollection<Product> Products { get; set; }

        public bool CheckAvailability(Cart cart, Product product)
        {
            foreach (var prod in cart.Products)
            {
                if (prod.Equals(product))
                {
                    prod.Amount++;
                    return true;
                }
            }

            return false;
        } 
    }
}