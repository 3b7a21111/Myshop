using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Utilities;
using Myshop.DataAccess.Implementation;
using Myshop.Entities.Models;
using Myshop.Entities.Repositories;
using Myshop.Entities.ViewModels;
using Stripe;

namespace Myshop.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetData()
        {
            IEnumerable<OrderHeader> orderHeaders = unitOfWork.OrderHeader.GetAll(IncludeWord: "ApplicationUser");
            return Json(new { data = orderHeaders });
        }
        
        public IActionResult Details (int orderid)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = unitOfWork.OrderHeader.GetFirstOrDefault
                                    (o=>o.Id==orderid,IncludeWord: "ApplicationUser"),
                OrderDetails = unitOfWork.OrderDetail.GetAll(o=>o.OrderHeaderId==orderid,IncludeWord:"Product")
            };
            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails ()
        {
            var orderfromdb = unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id);
            orderfromdb.Name = OrderVM.OrderHeader.Name;
			orderfromdb.Phone = OrderVM.OrderHeader.Phone;
			orderfromdb.Address = OrderVM.OrderHeader.Address;
			orderfromdb.City = OrderVM.OrderHeader.City;

            if(OrderVM.OrderHeader.Carrier != null)
            {
                orderfromdb.Carrier = OrderVM.OrderHeader.Carrier;
            }

            if(OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderfromdb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            unitOfWork.OrderHeader.Update(orderfromdb);
            unitOfWork.Complete();
            TempData["Update"] = "Item has Updated successfully";
            return RedirectToAction("Details", "Order", new {orderid = orderfromdb.Id});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProccess()
        {
            unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.Proccessing, null);
            unitOfWork.Complete();

			TempData["Update"] = "Order Status has Updated successfully";
            return RedirectToAction("Details", "Order", new {orderid = OrderVM.OrderHeader.Id});

		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartShip()
        {
            var orderfromdb = unitOfWork.OrderHeader.GetFirstOrDefault(o=>o.Id==OrderVM.OrderHeader.Id);
            orderfromdb.TrackingNumber=OrderVM.OrderHeader.TrackingNumber;
            orderfromdb.Carrier = OrderVM.OrderHeader.Carrier;
            orderfromdb.OrderStatus = SD.Shipped;
            orderfromdb.ShippingDate = DateTime.Now;

            unitOfWork.OrderHeader.Update(orderfromdb);
            unitOfWork.Complete();

			TempData["Update"] = "Order has Shipped successfully";
			return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            var orderfromdb = unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id);
            //هنا الحساب بتاع العميل يكفي انه يعمل اوردر بس لسبب ما الاوردر هيتلغي ولازم ارجع الفلوس للعميل 
            if (orderfromdb.PaymentStatus == SD.Approve)
            {
                var option = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderfromdb.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(option);

                unitOfWork.OrderHeader.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Refund);
            }
            else
            {
                unitOfWork.OrderHeader.UpdateStatus(orderfromdb.Id, SD.Cancelled, SD.Cancelled);
            }
            unitOfWork.Complete();

			TempData["Update"] = "Order has Cancelled successfully";
			return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
		}
	}
}
