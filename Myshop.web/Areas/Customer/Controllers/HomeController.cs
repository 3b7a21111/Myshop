using Microsoft.AspNetCore.Mvc;
using Myshop.Entities.Repositories;
using Myshop.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Myshop.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products = unitOfWork.Product.GetAll();
            return View(products);
        }
        public IActionResult Details(int ProductId)
        {
            ShoppingCart obj = new ShoppingCart()
            {
                ProductId = ProductId,
                Product = unitOfWork.Product.GetFirstOrDefault(x => x.Id == ProductId, IncludeWord: "Category"),
                Count = 1
            };
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
		public IActionResult Details(ShoppingCart shoppingCart)
		{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart Cartobj = unitOfWork.ShoppingCart.GetFirstOrDefault
                (u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

            if (Cartobj == null)
            {
                unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            else
            {
                unitOfWork.ShoppingCart.IncreaseCount(Cartobj, shoppingCart.Count);
            }

            unitOfWork.Complete();

            return RedirectToAction("Index");
		}
	}
}
