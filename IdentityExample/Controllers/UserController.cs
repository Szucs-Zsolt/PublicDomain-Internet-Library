using IdentityExample.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityExample.Controllers
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
        public IActionResult Index()
        {
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
                    user.RoleName = roles.FirstOrDefault(u => u.Id == userFirstRole.RoleId).Name;
                }

            }
            return View(users);
        }
    }
}
