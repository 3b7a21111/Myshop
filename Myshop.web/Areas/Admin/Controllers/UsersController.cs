using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Utilities;
using Myshop.DataAccess.Data;
using System.Security.Claims;

namespace Myshop.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext context;

        public UsersController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            //انا عايز اجيب كل ال users ماعدا اللي انا عامل بيه login 

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            return View(context.ApplicationUsers.Where(x=>x.Id != userId).ToList());
        }
        public IActionResult LockUnlock(string ? id)
        {
            var user = context.ApplicationUsers.FirstOrDefault(x => x.Id==id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.LockoutEnd== null || user.LockoutEnd <DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(1);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;

			}
            context.SaveChanges();
            return RedirectToAction("Index","Users",new {area="Admin"});
        }

	}
}
