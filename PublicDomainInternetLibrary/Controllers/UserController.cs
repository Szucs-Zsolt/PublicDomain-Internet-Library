using PublicDomainInternetLibrary.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PublicDomainInternetLibrary.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [Authorize(Roles = "Admin")]
        public IActionResult ChangeUserPassword(string email)
        {
            ChangeUserPasswordViewModel model = new ChangeUserPasswordViewModel()
            {
                UserEmail = email
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("****************************************************");
                Console.WriteLine($"model.UserEmail: {model.UserEmail}");
                Console.WriteLine($"model.NewPassword: {model.NewPassword}");
                Console.WriteLine("****************************************************");


                var user = await _userManager.FindByEmailAsync(model.UserEmail);
                if (user == null)
                {
                    ModelState.AddModelError("", "Nem találom a felhasználót az adatbázisban");
                    return View(model);
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token,
                    model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Description.Contains("Passwords must"))
                        {
                            ModelState.AddModelError("NewPassword",
                                "A jelszó legalább 6 karakter hosszú legyen, tartalmazzon kis és nagybetűt, számot és speciális karaktert is.");
                        }
                        else
                        {
                            // Általános hibaleírás, nem egy adott property-hez tartozik
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }

                // ModelState.IsValid, és result is OK

                return RedirectToAction("Index", "User");
            }

            // ModelState nem volt valid
            TempData["Success"] = $"{model.UserEmail} jelszava megváltoztatva.";
            return View(model);
        }

    }
}
