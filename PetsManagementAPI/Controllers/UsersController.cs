using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsManagementAPI.Data;
using PetsManagementAPI.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetsManagementAPI.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // Login Actions
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            string hashedPassword = HashPassword(password);

            // Check admin first
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username.ToLower() == username.ToLower() && a.PasswordHash == hashedPassword);
            if (admin != null)
            {
                await SignInUser(admin.AdminID, admin.Username, "Admin");
                return RedirectToAction("Index", "Home");
            }

            // Check regular user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.PasswordHash == hashedPassword);
            if (user != null)
            {
                await SignInUser(user.UserID, user.Username, "User");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // Signup/Register Actions
        [HttpGet("Signup")]
        [HttpGet("Register")]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost("Signup")]
        [HttpPost("Register")]
        public async Task<IActionResult> Signup(string username, string email, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "All fields are required.";
                return View();
            }

            try
            {
                if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
                {
                    TempData["Error"] = "Username already exists.";
                    return View();
                }
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
                {
                    TempData["Error"] = "Email already exists.";
                    return View();
                }

                string hashedPassword = HashPassword(password);
                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = hashedPassword,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Account created successfully! Please log in.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to create account: {ex.Message}";
                return View();
            }
        }

        // Logout Action
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ====================== ADOPTION REQUESTS ====================== //

        [Authorize]
        [HttpGet("ViewMyAdoptionRequest")]
        public async Task<IActionResult> ViewMyAdoptionRequest()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login");
            }

            var userID = int.Parse(userIdClaim.Value);
            var requests = await _context.AdoptionRequests
                .Where(r => r.UserID == userID)
                .Include(r => r.Pet)
                .ToListAsync();

            return View("~/Views/Users/ViewMyAdoptionRequest.cshtml", requests);
        }

        [Authorize]
        [HttpPost("Adoption/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdoptionRequest(int PetID, string FullName, string Address, string PhoneNumber)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login");
            }

            var userID = int.Parse(userIdClaim.Value);
            var request = new AdoptionRequest
            {
                UserID = userID,
                PetID = PetID,
                FullName = FullName,
                Address = Address,
                PhoneNumber = PhoneNumber,
                Status = "Pending",
                SubmittedAt = DateTime.Now
            };

            try
            {
                _context.AdoptionRequests.Add(request);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Adoption request submitted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to submit adoption request: {ex.Message}";
            }

            return RedirectToAction(nameof(ViewMyAdoptionRequest));
        }

        // Helper Methods
        private async Task SignInUser(int userId, string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = false // Ensure session-only cookie
            });
        }

        private string HashPassword(string password)
        {
            return password; // TODO: Replace with proper hashing (e.g., BCrypt) in production
        }
    }
}