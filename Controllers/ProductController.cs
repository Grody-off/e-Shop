using System.Linq;
using System.Threading.Tasks;
using e_Shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_Shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public static Product NewProduct(Product product)
        {
            var prod = new Product()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Amount = 1,
                Show = true
            };
            return prod;
        }
        
        public IActionResult Create() => View();

        [HttpPost]
        [Route("product/create")]
        public async Task<IActionResult> Create(Product product)
        {
            var prod = NewProduct(product);
            await _context.AddAsync(prod);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProductList");
        }

        
        [HttpGet,Route("product/edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            var prod = NewProduct(product);
            return View(prod);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product edit, int id)
        {
            if (edit == null)
                RedirectToAction("Index", "Home");
            
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            product.Name = edit.Name;
            product.Description = edit.Description;
            product.Price = edit.Price;
            product.Amount = edit.Amount;
            
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductList");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            _context.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductList");
        }
        [Route("product/productlist")]
        public IActionResult ProductList()
        {
            return View(_context.Products.ToList());
        }
    }
}