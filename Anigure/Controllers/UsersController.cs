using Anigure.Data;
using Anigure.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anigure.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<string, string?> _roles;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
            _roles = _context.Roles.AsNoTracking().ToDictionary(f => f.Id, f => f.Name);
        }


        public async Task<IActionResult> Index()
        {
            var model = new UserViewModel
            {
                Users = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Roles)
                    .ToListAsync(),
            };

            ViewBag.Roles = _roles;

            return View(model);
        }
    }
}
