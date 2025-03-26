using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsManagementAPI.Data;
using PetsManagementAPI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PetsManagementAPI.Controllers
{
    [Authorize(Roles = "Admin")] // ✅ Only admins can access
    public class AdminsController : Controller
    {
        private readonly DataContext _context; // ✅ Inject database context

        public AdminsController(DataContext context)
        {
            _context = context;
        }

        // ========================== USER MANAGEMENT ========================== //

        public async Task<IActionResult> ViewUsers()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int UserID, string Username, string Email)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data submitted.";
                return RedirectToAction(nameof(ViewUsers));
            }

            var user = await _context.Users.FindAsync(UserID);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(ViewUsers));
            }

            user.Username = Username;
            user.Email = Email;

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User updated successfully!";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Failed to update user: {ex.Message}";
            }

            return RedirectToAction(nameof(ViewUsers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int UserID)
        {
            var user = await _context.Users.FindAsync(UserID);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(ViewUsers));
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User deleted successfully!";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Failed to delete user: {ex.Message}";
            }

            return RedirectToAction(nameof(ViewUsers));
        }


        // ========================== PET MANAGEMENT ========================== //

        // ✅ Fetch all pets (ViewPets.cshtml)
        public async Task<IActionResult> ViewPets()
        {
            var pets = await _context.Pets.ToListAsync();
            return View(pets);
        }

        // ✅ Create Pet (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePet(string PetName, string PetType, string Breed, int Age, string Status, bool Neutered, bool Vaccine)
        {
            var pet = new Pet
            {
                PetName = PetName,
                PetType = PetType,
                Breed = Breed,
                Age = Age,
                Status = Status,
                Neutered = Neutered,
                Vaccine = Vaccine,
                AddedAt = DateTime.Now
            };

            try
            {
                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Pet added successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to add pet: {ex.Message}";
            }
            return RedirectToAction(nameof(ViewPets));
        }

        // ✅ Edit Pet (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPet(int PetID, string PetName, string PetType, string Breed, int Age, string Status, bool Neutered, bool Vaccine)
        {
            var pet = await _context.Pets.FindAsync(PetID);
            if (pet == null)
            {
                TempData["Error"] = "Pet not found.";
                return RedirectToAction(nameof(ViewPets));
            }

            pet.PetName = PetName;
            pet.PetType = PetType;
            pet.Breed = Breed;
            pet.Age = Age;
            pet.Status = Status;
            pet.Neutered = Neutered;
            pet.Vaccine = Vaccine;

            try
            {
                _context.Update(pet);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Pet updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to update pet: {ex.Message}";
            }
            return RedirectToAction(nameof(ViewPets));
        }

        // ✅ Delete Pet (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePet(int PetID)
        {
            var pet = await _context.Pets.FindAsync(PetID);
            if (pet == null)
            {
                TempData["Error"] = "Pet not found.";
                return RedirectToAction(nameof(ViewPets));
            }

            try
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Pet deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete pet: {ex.Message}";
            }
            return RedirectToAction(nameof(ViewPets));
        }

        // ===================== ADOPTION REQUEST MANAGEMENT ===================== //

        // View All Adoption Requests (Admin Only)
        public async Task<IActionResult> ViewAdoptionRequest()
        {
            var requests = await _context.AdoptionRequests
                .Include(r => r.Pet)
                .Include(r => r.User)
                .ToListAsync();
            return View(requests);
        }

        [HttpPost]
public IActionResult UpdateStatus(int AdoptID, string Status)
{
    var adoptionRequest = _context.AdoptionRequests.FirstOrDefault(a => a.AdoptID == AdoptID);
    
    if (adoptionRequest == null)
    {
        TempData["Error"] = "Adoption request not found.";
        return RedirectToAction("ViewAdoptionRequest");
    }

    // Update adoption request status
    adoptionRequest.Status = Status;
    _context.SaveChanges();

    // If approved, update pet status
    if (Status == "Approved")
    {
        var pet = _context.Pets.FirstOrDefault(p => p.PetID == adoptionRequest.PetID);
        if (pet != null)
        {
            pet.Status = "Adopted";
            _context.SaveChanges();
        }
    }

    TempData["Success"] = $"Adoption request has been {Status.ToLower()}!";
    return RedirectToAction("ViewAdoptionRequest");
}

        // Delete Adoption Request (Admin Only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int AdoptID)
        {
            var request = await _context.AdoptionRequests.FindAsync(AdoptID);
            if (request != null)
            {
                _context.AdoptionRequests.Remove(request);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Adoption request deleted!";
            }
            return RedirectToAction(nameof(ViewAdoptionRequest));
        }
    }
}
