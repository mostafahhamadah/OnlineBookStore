using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;    
        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {

            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            //what you are going to include the names must match !! * 
            var dbProducts = _unitOfWork.Product.GetAll(includeproperties:"category").ToList();
           
            return View(dbProducts);
        }

        public IActionResult upsert(int? id)
        {
            var dbCategories = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            ProductVM productVM = new()
            {
                CategoryList = dbCategories,
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create product
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult upsert(ProductVM obj,IFormFile? File)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (File != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(File.FileName);
                    //this will provide the path where i want to upload the file   
                    string productPath = Path.Combine(wwwRootPath, @"Images/Product");

                    if (!string.IsNullOrEmpty(obj.Product.ImageURL))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageURL.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }


                    }



                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        File.CopyTo(fileStream);
                    }
                    obj.Product.ImageURL = @"\Images\Product\" + fileName;
                }



                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.add(obj.Product);
                } else
                {
                    _unitOfWork.Product.update(obj.Product);

                }
                _unitOfWork.save();
                return RedirectToAction("Index");

            }else
            {
                 obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }
           
        }

   
     
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() {
            var dbProducts = _unitOfWork.Product.GetAll(includeproperties: "category").ToList();
               return Json (new { data= dbProducts });
        }


        [HttpDelete]
        public IActionResult Delete(int ? id) {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleteing"});
            }
            var OldImagePath =
                                Path.Combine(_webHostEnvironment.WebRootPath
                          , productToBeDeleted.ImageURL.TrimStart('\\'));
        
          if (System.IO.File.Exists(OldImagePath))
            {
                System.IO.File.Delete(OldImagePath);
            }
            _unitOfWork.Product.remove(productToBeDeleted);
            _unitOfWork.save();


            return Json(new { success = true, message = " Deleted Successfully" });
        }
        #endregion

    }
}
