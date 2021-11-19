using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using e_Shop.Models;
using e_Shop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace e_Shop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public CartController(AppDbContext context, IHttpContextAccessor httpContextAccessor,UserManager<IdentityUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userManager = userManager;
        }

        private async Task<Cart> GetCart()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts.Include(p => p.Products).FirstOrDefaultAsync(c => c.UserId == userId);
            return cart;
        }

        public async Task<IActionResult> CartList() => View(await GetCart());

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            var cart = await GetCart();
            var cartProduct = ProductController.NewProduct(product);
            cartProduct.Show = false;

            if (!cart.CheckAvailability(cart, product))
                cart.Products.Add(cartProduct);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var cart = await GetCart();
            var prod = cart.Products.FirstOrDefault(p => p.Id == id);
            if (prod == null)
                return NotFound();
            cart.Products.Remove(prod);
            _context.Remove(prod);
            await _context.SaveChangesAsync();

            return RedirectToAction("CartList");
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var cart = await GetCart();
            if (cart.Products.Any())
            {
                var productForRemove = new List<Product>();
                foreach (var product in cart.Products)
                {
                    productForRemove.Add(product);
                }

                _context.RemoveRange(productForRemove);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CartList");
        }

        public async Task<IActionResult> MakeOrder()
        {
            var emailService = new EmailService();
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            var cart = await GetCart();
            var prod = string.Empty;
            var productForRemove = new List<Product>();
            foreach (var product in cart.Products)
            {
                prod += $"<h3><b>{product.Name}, id: {product.Id}, amount: {product.Amount}</b></h3><br>";
                productForRemove.Add(product);
            }
            await emailService.SendEmailAsync("mr.payne0007@gmail.com", $"Ordering products by the user {user.Email}",  prod);

            _context.RemoveRange(productForRemove);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index", "Home");
        }
        
        
        #region Update cart
        // [HttpGet, Route("cart/update")]
        // public async Task<IActionResult> Update()
        // {
        //     var cart = await GetCart();
        //     if (cart.Products.Any())
        //     {
        //         var editCart = new Cart {UserId = null, Products = cart.Products};
        //         if (editCart.Products == null)
        //         {
        //             return RedirectToAction("CartList");
        //         }
        //         return View(editCart);
        //     }
        //
        //     return RedirectToAction("CartList");
        // }
        //
        // [HttpPost]
        // public async Task<IActionResult> Update(Cart edit)
        // {
        //     if (edit.Products == null) 
        //         return NotFound();
        //     var cart = await GetCart();
        //     foreach (var editProduct in edit.Products )
        //     {
        //         var prod = cart.Products.FirstOrDefault(product => product.Id == editProduct.Id);
        //         if (prod == null)
        //             return NotFound();
        //         
        //         prod.Amount = editProduct.Amount;
        //         
        //         _context.Products.Update(prod);
        //         await _context.SaveChangesAsync();
        //     }
        //
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction("CartList");
        // }
        #endregion
    }
}