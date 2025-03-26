using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsManagementAPI.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PetManagement.Controllers
{
    public class PetsController : Controller
    {
        private readonly DataContext _context;

        public PetsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> FindPet(string petType, string breed, int? age)
        {
            var query = _context.Pets.Where(p => p.Status == "Available").AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(petType))
            {
                query = query.Where(p => p.PetType.ToLower() == petType.ToLower());
            }
            if (!string.IsNullOrEmpty(breed))
            {
                query = query.Where(p => p.Breed != null && p.Breed.ToLower().Contains(breed.ToLower()));
            }
            if (age.HasValue)
            {
                query = query.Where(p => p.Age <= age.Value);
            }

            var pets = await query.ToListAsync();
            return View(pets);
        }
    }
}