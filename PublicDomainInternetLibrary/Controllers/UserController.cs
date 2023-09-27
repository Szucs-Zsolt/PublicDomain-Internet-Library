using PublicDomainInternetLibrary.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PublicDomainInternetLibrary.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PublicDomainInternetLibrary.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager; 
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            UserIndexViewModel viewModel = new UserIndexViewModel();

            // összes user beolvasása
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                // egy usernek csak egy role-ja lehet, ezért csak az első érdekel
                var userRoles = await _userManager.GetRolesAsync(user);
                string userFirstRole = userRoles.First();
                // role alapján szétosztjuk két különböző táblába
                if (userFirstRole == "Librarian")
                    viewModel.LibrarianList.Add(user);
                else if (userFirstRole == "User")
                    viewModel.UserList.Add(user);
            }

            viewModel.LibrarianList = viewModel.LibrarianList.OrderBy(x => x.Email).ToList();
            viewModel.UserList = viewModel.UserList.OrderBy(x => x.Email).ToList();
            return View(viewModel);
/*
            // AspNet utáni rész: AspNetRoles -> .Roles
            var users = _db.ApplicationUsers.ToList();  // (user) Id
            var roles = _db.Roles.ToList();             // (role) Id + Name
            var userRoles = _db.UserRoles.ToList();     // UserId + RoleId (!)

            foreach(var user in users)
            {
                var userFirstRole = userRoles.FirstOrDefault(u => u.UserId == user.Id);
                if (userFirstRole == null)
                {
                    user.RoleName = "nincs";
                } else
                {
                    var role = roles.FirstOrDefault(u => u.Id == userFirstRole.RoleId);
                    if (role != null)
                        user.RoleName = role.Name;
                    else
                        user.RoleName = "nincs";
                }

            }
            return View(users);
*/
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ChangeLibrarianRoleStatus(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return RedirectToAction("Index", "User");
            }
             
            IList<string> currentRoles = await _userManager.GetRolesAsync(user);
            // Ha eddig könyvtáros volt, elvesszük a jogot, ha nem, megadjuk neki
            if (currentRoles.Contains("Librarian")) 
            {
                await _userManager.RemoveFromRoleAsync(user, "Librarian");
                await _userManager.AddToRoleAsync(user, "User");
            } else
            {
                await _userManager.RemoveFromRoleAsync(user, "User");
                await _userManager.AddToRoleAsync(user, "Librarian");
            }


            return RedirectToAction("Index");
        }
    }
}
