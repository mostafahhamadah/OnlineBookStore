using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeproperties: "category");
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {

                product = _unitOfWork.Product.Get(u => u.Id == id, includeproperties: "category"),
                Count = 1,
                ProductId= id
        };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            shoppingCart.ApplicationUserId = userId;
            shoppingCart.ApplicationUser = _unitOfWork.ApplicationUser.Get(u=>u.Id==userId);

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && 
                    u.ProductId == shoppingCart.ProductId);
            if (cartFromDb!= null)
            {
                cartFromDb.Count += shoppingCart.Count;
               
                _unitOfWork.ShoppingCart.update(cartFromDb);
            }
            else
            {
                _unitOfWork.ShoppingCart.add(shoppingCart);
            }
            
            TempData["Success"] = "Cart Updated Successfully";
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}