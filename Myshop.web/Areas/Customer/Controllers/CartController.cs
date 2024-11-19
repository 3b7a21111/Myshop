using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Myshop.Entities.Models;
using Myshop.Entities.Repositories;
using Myshop.Entities.ViewModels;
using System.Security.Claims;

namespace Myshop.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public int TotalCarts { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartsList = unitOfWork.ShoppingCart.GetAll
                (u => u.ApplicationUserId == claim.Value,IncludeWord:"Product")
            };

            foreach(var item in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.TotalCarts += (item.Count * item.Product.Price); 
            }

            return View(ShoppingCartVM);
        }
        public IActionResult Plus (int cartid)
        {
            var shoppingcart = unitOfWork.ShoppingCart.GetFirstOrDefault(x=>x.Id==cartid);
            unitOfWork.ShoppingCart.IncreaseCount(shoppingcart, 1);
            unitOfWork.Complete();
            return RedirectToAction("Index");
        }
        public IActionResult Minus (int cartid)
        {
            var shoppingcart = unitOfWork.ShoppingCart.GetFirstOrDefault(x=>x.Id == cartid);
            if(shoppingcart.Count <= 1)
            {
                unitOfWork.ShoppingCart.Remove(shoppingcart);
                unitOfWork.Complete();
                return RedirectToAction("Index","Home");
            }
            else
            {
				unitOfWork.ShoppingCart.DecreaseCount(shoppingcart, 1);
			}
			unitOfWork.Complete();
            return RedirectToAction("Index");

        }
        public IActionResult Remove(int cartid)
        {
            var shoppingcart = unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartid);
            unitOfWork.ShoppingCart.Remove(shoppingcart);
            unitOfWork.Complete();
            return RedirectToAction("Index");
        }
    }
}
