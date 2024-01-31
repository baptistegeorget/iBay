using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Utils;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public UsersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
        {
            var users = await _context.Users
                .Select(user => new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Pseudo = user.Pseudo,
                    Role = user.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userResponse = new UserResponse
            {
                Id=user.Id,
                Email = user.Email,
                Pseudo = user.Pseudo,
                Role = user.Role
            };

            return Ok(userResponse);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(long id, UserRequest userRequest)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (id.ToString() != userId)
            {
                return Unauthorized();
            }

            var user = new User
            {
                Id = id,
                Email = userRequest.Email,
                Pseudo = userRequest.Pseudo,
                PasswordHash = Password.Hash(userRequest.Password),
                Role = userRequest.Role
            };

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserRequest userRequest)
        {
            var user = new User
            { 
                Email = userRequest.Email,
                Pseudo = userRequest.Pseudo,
                PasswordHash = Password.Hash(userRequest.Password),
                Role = userRequest.Role
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Pseudo = user.Pseudo,
                Role = user.Role
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userResponse);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id.ToString() != userId)
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/Cart")]
        [Authorize]
        public async Task<ActionResult<CartResponse>> GetCart(long id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id.ToString() != userId)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(user => user.Carts)
                    .ThenInclude(cart => cart.CartProducts)
                        .ThenInclude(cartProduct => cartProduct.Product)
                .FirstOrDefaultAsync(user => user.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var nonValidatedCart = user.Carts.FirstOrDefault(cart => !cart.IsValidate);

            if (nonValidatedCart == null)
            {
                nonValidatedCart = new Cart { UserId = user.Id, User = user };

                _context.Carts.Add(nonValidatedCart);

                await _context.SaveChangesAsync();
            }

            var cartResponse = new CartResponse
            {
                Id = nonValidatedCart.Id,
                IsValidate = nonValidatedCart.IsValidate,
                ProductsId = nonValidatedCart.CartProducts.Select(cartProduct => cartProduct.Product.Id).ToArray(),
                Quantities = nonValidatedCart.CartProducts.ToDictionary(cartProduct => cartProduct.Product.Id, cartProduct => cartProduct.Quantity)
            };

            return Ok(cartResponse);
        }

        [HttpPost("{id}/Cart")]
        [Authorize]
        public async Task<ActionResult> AddProductToCart(long id, long productId, int quantity = 1)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id.ToString() != userId)
            {
                return Unauthorized();
            }

            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            if (!product.IsAvailable)
            {
                return BadRequest();
            }

            var user = await _context.Users
                .Include(user => user.Carts)
                    .ThenInclude(cart => cart.CartProducts)
                .FirstOrDefaultAsync(user => user.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var nonValidatedCart = user.Carts.FirstOrDefault(cart => !cart.IsValidate);

            if (nonValidatedCart == null)
            {
                nonValidatedCart = new Cart { UserId = user.Id, User = user };

                _context.Carts.Add(nonValidatedCart);

                await _context.SaveChangesAsync();
            }

            var existingCartItem = nonValidatedCart.CartProducts.FirstOrDefault(cartProduct => cartProduct.ProductId == product.Id);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                nonValidatedCart.CartProducts.Add(new CartProduct { Product = product, Quantity = quantity, Cart = nonValidatedCart });
            }

            await _context.SaveChangesAsync();

            await _context.Entry(nonValidatedCart)
                .Collection(cart => cart.CartProducts)
                .Query()
                .Include(cartProduct => cartProduct.Product)
                .LoadAsync();

            var cartResponse = new CartResponse
            {
                Id = nonValidatedCart.Id,
                IsValidate = nonValidatedCart.IsValidate,
                ProductsId = nonValidatedCart.CartProducts.Select(cartProduct => cartProduct.Product.Id).ToArray(),
                Quantities = nonValidatedCart.CartProducts.ToDictionary(cartProduct => cartProduct.Product.Id, cartProduct => cartProduct.Quantity)
            };

            return Ok(cartResponse);
        }

        [HttpDelete("{id}/Cart")]
        [Authorize]
        public async Task<ActionResult> RemoveProductFromCart(long id, long productId, int quantity = 1)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id.ToString() != userId)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(user => user.Carts)
                    .ThenInclude(cart => cart.CartProducts)
                .FirstOrDefaultAsync(user => user.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var nonValidatedCart = user.Carts.FirstOrDefault(cart => !cart.IsValidate);

            if (nonValidatedCart == null)
            {
                nonValidatedCart = new Cart { UserId = user.Id, User = user };

                _context.Carts.Add(nonValidatedCart);

                await _context.SaveChangesAsync();
            }

            var cartProductToRemove = nonValidatedCart.CartProducts.FirstOrDefault(cartProduct => cartProduct.ProductId == productId);

            if (cartProductToRemove == null)
            {
                return NotFound();
            }

            cartProductToRemove.Quantity -= quantity;

            if (cartProductToRemove.Quantity <= 0)
            {
                _context.CartProducts.Remove(cartProductToRemove);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/Cart/Pay")]
        [Authorize]
        public async Task<ActionResult> PayCart(long id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (id.ToString() != userId)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(user => user.Carts)
                    .ThenInclude(cart => cart.CartProducts)
                        .ThenInclude(cartProduct => cartProduct.Product)
                .FirstOrDefaultAsync(user => user.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var nonValidatedCart = user.Carts.FirstOrDefault(cart => !cart.IsValidate);

            if (nonValidatedCart == null || nonValidatedCart.CartProducts.Count == 0)
            {
                return BadRequest();
            }

            // Logique de paiement ici...

            nonValidatedCart.IsValidate = true;

            var newCart = new Cart { UserId = user.Id, User = user };

            _context.Carts.Add(newCart);

            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
