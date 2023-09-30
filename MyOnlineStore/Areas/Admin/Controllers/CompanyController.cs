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

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
            
        }
        public IActionResult Index()
        {
            //what you are going to include the names must match !! * 
            var dbCompanies = _unitOfWork.Company.GetAll().ToList();
           
            return View(dbCompanies);
        }

        public IActionResult upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //create Company
                return View(new Company());
            }
            else
            {
                //update
                Company companyObj = _unitOfWork.Company.Get(u=>u.Id==id);
                return View(companyObj);
            }
        }
        [HttpPost]
        public IActionResult upsert(Company CompanyObj)
        {

            if (ModelState.IsValid)
            {
               

                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.add(CompanyObj);
                } else
                {
                    _unitOfWork.Company.update(CompanyObj);

                }
                _unitOfWork.save();
                return RedirectToAction("Index");

            }else
            {
                
                return View(CompanyObj);
            }
           
        }

   
     
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() {
            var dbCompanys = _unitOfWork.Company.GetAll().ToList();
               return Json (new { data= dbCompanys });
        }


        [HttpDelete]
        public IActionResult Delete(int ? id) {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleteing"});
            }
           
            _unitOfWork.Company.remove(CompanyToBeDeleted);
            _unitOfWork.save();


            return Json(new { success = true, message = " Deleted Successfully" });
        }
        #endregion

    }
}
