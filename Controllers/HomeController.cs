using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using vnenterprises.Models;
using vnenterprises.Models;
using vnenterprises.Support;
namespace vnenterprises.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly UserSupport _usersupport;


        public HomeController(ILogger<HomeController> logger , UserSupport usersupport)
        {
            _logger = logger;
            _usersupport = usersupport;
        }

        public IActionResult Index()
        {
            return View(new AccountModelHandle());
        }
        [HttpPost]
        public IActionResult LogOn(AccountModelHandle LogOnmodel)
        {
            if(LogOnmodel.login.Password != null)
            {
                LogOnmodel.login.Password = ConvertToBase64(LogOnmodel.login.Password);
            }
            var user = _usersupport.GetUserAccess(LogOnmodel);
            if (user.UserId > 0)
            {
                var userCookie = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.Now.AddHours(2),
                    SameSite = SameSiteMode.Strict,
                    Secure = true
                };

                Response.Cookies.Append("UserId", user.UserId.ToString(), userCookie);
                Response.Cookies.Append("AccessType", user.AccessType.ToString(), userCookie);

                if (user.AccessType == 1)
                    return RedirectToAction("Index", "Admin");
                if (user.AccessType == 2)
                    return RedirectToAction("Index", "Manager");
                if (user.AccessType == 3)
                    return RedirectToAction("Customers", "Employee");
                if (user.AccessType == 4)
                    return RedirectToAction("Customers", "Retailer");
                else
                    return RedirectToAction("Index", "Home");

            }
            else
            {
                TempData["LogInError"] = "Invalid phone number, password or MPIN.";
                ViewBag.ActiveForm = "LoginForm";
                return RedirectToAction("Index", "Home");
            }

        }

        public string ConvertToBase64(string plainPassword)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return Convert.ToBase64String(plainTextBytes);
        }
        [HttpPost]
        public IActionResult MPin(AccountModelHandle accountmodel)
        {
            if (accountmodel.mpin.Password != null)
            {
                accountmodel.mpin.Password = ConvertToBase64(accountmodel.mpin.Password);
            }
            var UserId =  Convert.ToInt32(Request.Cookies["UserId"]);
            var result = _usersupport.UpdateMpin(accountmodel , UserId);
            if(result > 0)
            {
                TempData["LogInError"] = "MPIN updated successfully";
                return RedirectToAction("Index");
            }
            TempData["MpinError"] = "Incorrect Mobile number or Password.";         
            ViewBag.ActiveForm = "MpinForm";
            return View("Index", accountmodel);
        }
        [HttpPost]
        public IActionResult ForgotPassword(AccountModelHandle accountmodel)
        {
            if(accountmodel.forgotpassword.Password != null){
                accountmodel.forgotpassword.Password = ConvertToBase64(accountmodel.forgotpassword.Password);
            }
            var UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            var result = _usersupport.UpdatePassword(accountmodel , UserId);

            if (result > 0)
            {
                TempData["LogInError"] = "Password updated successfully";
                return RedirectToAction("Index");
            }
            TempData["PasswordError"] = "Incorrect Mobilr number or MPIN.";
            ViewBag.ActiveForm = "ForgotPasswordForm";
            return View("Index", accountmodel);
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
