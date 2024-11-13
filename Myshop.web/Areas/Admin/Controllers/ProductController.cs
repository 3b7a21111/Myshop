using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Myshop.DataAccess.Data;
using Myshop.Entities.Models;
using Myshop.Entities.Repositories;
using Myshop.Entities.ViewModels;


namespace Myshop.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
		private readonly IWebHostEnvironment webHost;

		public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHost)
        {
            this.unitOfWork = unitOfWork;
			this.webHost = webHost;
		}
        public IActionResult Index()
        {
            return View();
        }
		public IActionResult GetData()
		{
            var products = unitOfWork.Product.GetAll(IncludeWord: "Category");
            return Json(new {data = products});
		}
		[HttpGet]
        public IActionResult Create()
        {
            var productvm = new ProductVM()
            {
                Product = new Product(),
				CategoryIds = unitOfWork.Category.GetAll().Select(x => new SelectListItem
				{
					Text = x.Name,
					Value = x.Id.ToString()
				})
			};
            
            return View(productvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM  productVM , IFormFile file )
        {
			
			if (ModelState.IsValid)
            {
                string RootPath = webHost.WebRootPath;
                if(file != null)
                {
                    string FileName = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Products");
                    var Extension = Path.GetExtension(file.FileName);

                    using (var filestream = new FileStream(Path.Combine(Upload,FileName+Extension),FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
					productVM.Product.Image = @"Images\Products\" + FileName + Extension;

				}
				//context.Categories.Add(product);
				unitOfWork.Product.Add(productVM.Product);
                //context.SaveChanges();
                unitOfWork.Complete();
                TempData["Create"] = "Data Has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null | id == 0)
            {
                return NotFound();
            }

			var productvm = new ProductVM()
			{
				Product = unitOfWork.Product.GetFirstOrDefault(x => x.Id == id),
				CategoryIds = unitOfWork.Category.GetAll().Select(x => new SelectListItem
				{
					Text = x.Name,
					Value = x.Id.ToString()
				})
			};

			return View(productvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
				string RootPath = webHost.WebRootPath;
				if (file != null)
				{
					string FileName = Guid.NewGuid().ToString();
					var Upload = Path.Combine(RootPath, @"Images\Products");
					var Extension = Path.GetExtension(file.FileName);
                    if(productVM.Product.Image != null)
                    {
                        var oldImage = Path.Combine(RootPath, productVM.Product.Image.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }

					using (var filestream = new FileStream(Path.Combine(Upload, FileName + Extension), FileMode.Create))
					{
						file.CopyTo(filestream);
					}
					productVM.Product.Image = @"Images\Products\" + FileName + Extension;

				}
				unitOfWork.Product.Update(productVM.Product);
                unitOfWork.Complete();
                TempData["Update"] = "Data Has Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }
       
        [HttpDelete]
        public IActionResult DeleteProduct (int? id)
        {
            var productfromdb = unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            if (productfromdb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            //context.Categories.Remove(product fromdb!);
            unitOfWork.Product .Remove(productfromdb);
			var oldImage = Path.Combine(webHost.WebRootPath, productfromdb.Image.TrimStart('\\'));
			if (System.IO.File.Exists(oldImage))
			{
				System.IO.File.Delete(oldImage);
			}
			//context.SaveChanges();
			unitOfWork.Complete();
			return Json(new { success = true, message = "File has been Deleted" });
			//TempData["Delete"] = "Data Has Deleted Successfully";
			//return RedirectToAction("Index");
		}
    }
}
