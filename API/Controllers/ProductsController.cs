using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static API.Models.User;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ProductsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
        {
            var products = await _context.Products
                .Select(product => new ProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Image = product.Image,
                    Price = product.Price,
                    IsAvailable = product.IsAvailable,
                    CreatedOn = product.CreatedOn,
                    UserId = product.UserId
                })
                .ToListAsync();
            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productResponse = new ProductResponse
            {
                Id= product.Id,
                Name = product.Name,
                Image = product.Image,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
                CreatedOn = product.CreatedOn,
                UserId = product.UserId
            };

            return Ok(productResponse);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProduct(long id, ProductRequest productRequest)
        {
            var userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdString == null || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.UserId != userId)
            {
                return Unauthorized();
            }

            product.Name = productRequest.Name;
            product.Image = productRequest.Image;
            product.Price = productRequest.Price;
            product.IsAvailable = productRequest.IsAvailable;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> PostProduct(ProductRequest productRequest)
        {
            var userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole != Roles.Vendor.ToString() || userIdString == null || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var product = new Product
            {
                Name = productRequest.Name,
                Image = productRequest.Image,
                Price = productRequest.Price,
                IsAvailable = productRequest.IsAvailable,
                UserId = userId,
                User = user
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            var productResponse = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Image = product.Image,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
                CreatedOn = product.CreatedOn,
                UserId = user.Id
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productResponse);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdString == null || !long.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.UserId != userId)
            {
                return Unauthorized();
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
