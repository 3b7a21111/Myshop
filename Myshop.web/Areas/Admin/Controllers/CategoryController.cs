using Microsoft.AspNetCore.Mvc;
using Myshop.DataAccess.Data;
using Myshop.Entities.Models;
using Myshop.Entities.Repositories;


namespace Myshop.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //var categories = context.Categories.ToList();
            var categories = unitOfWork.Category.GetAll();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                //context.Categories.Add(category);
                unitOfWork.Category.Add(category);
                //context.SaveChanges();
                unitOfWork.Complete();
                TempData["Create"] = "Data Has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null | id == 0)
            {
                return NotFound();
            }
            //var categoryfromdb = context.Categories.Find(id);
            var categoryfromdb = unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            return View(categoryfromdb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                //context.Categories.Update(category);
                unitOfWork.Category.Update(category);
                //context.SaveChanges();
                unitOfWork.Complete();
                TempData["Update"] = "Data Has Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null | id == 0)
            {
                NotFound();
            }
            var categoryfromdb = unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            return View(categoryfromdb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int? id)
        {
            var categoryfromdb = unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            if (categoryfromdb == null)
            {
                return NotFound("this category does not found");
            }
            //context.Categories.Remove(categoryfromdb!);
            unitOfWork.Category.Remove(categoryfromdb);
            //context.SaveChanges();
            unitOfWork.Complete();
            TempData["Delete"] = "Data Has Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
