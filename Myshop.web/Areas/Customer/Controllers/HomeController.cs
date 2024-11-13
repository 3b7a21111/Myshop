using Microsoft.AspNetCore.Mvc;
using Myshop.Entities.Repositories;
using Myshop.Entities.ViewModels;

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
        public IActionResult Details(int id)
        {
            ShoppingCart obj = new ShoppingCart()
            {
                Product = unitOfWork.Product.GetFirstOrDefault(x=>x.Id==id , IncludeWord:"Category"),
                Count = 1
            };
            return View(obj);
        }
    }
}
