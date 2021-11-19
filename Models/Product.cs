namespace e_Shop.Models
{
    public class Product 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public bool Show { get; set; }
        public override bool Equals(object value)
        {
            var product = value as Product;

            return !ReferenceEquals(null, product)
                   && string.Equals(Name, product.Name)
                   && string.Equals(Description, product.Description)
                   && Equals(Price, product.Price);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Description.GetHashCode();
        }
    }
}