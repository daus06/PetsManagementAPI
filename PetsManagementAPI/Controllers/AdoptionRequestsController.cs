using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetsManagementAPI.Data;
using PetsManagementAPI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PetsManagementAPI.Controllers
{
[Authorize] // Users must be logged in
public class AdoptionRequestController : Controller
{
    private readonly DataContext _context;

    public AdoptionRequestController(DataContext context)
    {
        _context = context;
    }

    // ✅ Users view their own adoption requests
    public async Task<IActionResult> ViewMyAdoptionRequest()
    {
        var userID = int.Parse(User.FindFirst("UserID").Value); // Get logged-in user ID
        var requests = await _context.AdoptionRequests
            .Where(r => r.UserID == userID)
            .Include(r => r.PetID)
            .ToListAsync();
        return View(requests);
    }

    // ✅ Users submit a new adoption request
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int PetID, string FullName, string Address, string PhoneNumber)
    {
        var userID = int.Parse(User.FindFirst("UserID").Value); // Get logged-in user ID

        var request = new AdoptionRequest
        {
            UserID = userID,
            PetID = PetID,
            FullName = FullName,
            Address = Address,
            PhoneNumber = PhoneNumber,
            Status = "Pending"
        };

        _context.AdoptionRequests.Add(request);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Adoption request submitted successfully!";
        return RedirectToAction(nameof(ViewMyAdoptionRequest));
    }
}
}