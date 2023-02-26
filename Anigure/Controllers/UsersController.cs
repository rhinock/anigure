using Anigure.Authorization;
using Anigure.Data;
using Anigure.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anigure.Controllers
{
    [Authorize(Roles = Roles.AdministratorsRole)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<string, string?> _roles;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
            _roles = _context.Roles.AsNoTracking().ToDictionary(f => f.Id, f => f.Name);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new UserIndexViewModel
            {
                Users = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Roles)
                    .ToListAsync(),

                Roles = _roles
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var model = new UserEditViewModel
            {
                User = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Roles)
                    .FirstAsync(u => u.Id == id),

                Roles = _roles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser user)
        {
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.IsBlocked, user.IsBlocked));

            await _context.SaveChangesAsync();

            var model = new UserEditViewModel
            {
                User = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Roles)
                    .FirstAsync(u => u.Id == id),

                Roles = _roles
            };

            return View(model);
        }
    }
}
