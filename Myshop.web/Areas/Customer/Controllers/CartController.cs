using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using myshop.Utilities;
using Myshop.Entities.Models;
using Myshop.Entities.Repositories;
using Myshop.Entities.ViewModels;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace Myshop.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;

        public ShoppingCartVM ShoppingCartVM { get; set; }
        public int TotalCarts { get; set; }
        public CartController(IUnitOfWork unitOfWork,IConfiguration configuration = null)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartsList = unitOfWork.ShoppingCart.GetAll
                (u => u.ApplicationUserId == claim.Value,IncludeWord:"Product"),
                OrderHeader = new()
            };

            
            foreach(var item in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price); 
            }

            return View(ShoppingCartVM);
        }
        [HttpGet]
        public IActionResult Summary()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				CartsList = unitOfWork.ShoppingCart.GetAll
				(u => u.ApplicationUserId == claim.Value, IncludeWord: "Product"),
                OrderHeader = new()
			};

			ShoppingCartVM.OrderHeader.ApplicationUser =
						unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);
			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.Phone = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;


			foreach (var item in ShoppingCartVM.CartsList)
			{
				ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
			}

			return View(ShoppingCartVM);
		}
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult POSTSummary(ShoppingCartVM shoppingCartVM)
        {
            var claimIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartVM.CartsList = 
                unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,IncludeWord:"Product");

            shoppingCartVM.OrderHeader.OrderStatus = SD.Pending;
            shoppingCartVM.OrderHeader.PaymentStatus = SD.Pending;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var item in shoppingCartVM.CartsList)
            {
                shoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            }

            unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            unitOfWork.Complete();

            foreach (var item in shoppingCartVM.CartsList)
            {
                var orderDetails = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    Price = item.Product.Price,
                    Count = item.Count,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id
                };
                unitOfWork.OrderDetail.Add(orderDetails);
                unitOfWork.Complete();
            }


            var domain = "https://localhost:5001/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain+$"Customer/Cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain+$"customer/cart/index",
            };

            foreach (var item in shoppingCartVM.CartsList)
            {
                var sessionlineoption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        }
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionlineoption);
            }

            var service = new SessionService();
            //var configure = configuration ?? HttpContext.RequestServices.GetService<IConfiguration>();
            //var stripeSecretKey = configure.GetSection("Stripe")["SecretKey"];
            //var stripeClient = new StripeClient(stripeSecretKey);
            //var service = new SessionService(stripeClient);
            Session session = service.Create(options);
            shoppingCartVM.OrderHeader.SessionId = session.Id;
            shoppingCartVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            unitOfWork.Complete();

            Response.Headers.Add("Locaion", session.Url);

            return Redirect(session.Url);


            //unitOfWork.ShoppingCart.REmoveRange(shoppingCartVM.CartsList);
            //unitOfWork.Complete();
            //return RedirectToAction("Index", "Home");
        }
        public IActionResult OrderConfirmation (int id)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            if(session.PaymentStatus.ToLower() == "paid")
            {
                unitOfWork.OrderHeader.UpdateStatus(id, SD.Approve, SD.Approve);
                //orderHeader.PaymentIntentId = session.PaymentIntentId;
                unitOfWork.Complete();
            }
            List<ShoppingCart> shoppingCarts = unitOfWork.ShoppingCart
                               .GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            unitOfWork.ShoppingCart.REmoveRange(shoppingCarts);
            unitOfWork.Complete();
            return View(id);

            //         unitOfWork.ShoppingCart.REmoveRange(shoppingCarts);
            //         unitOfWork.Complete();
            //return View(id);
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
