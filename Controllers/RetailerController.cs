using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using vnenterprises.Filters;
using vnenterprises.Models;
using vnenterprises.Support;

namespace vnenterprises.Controllers
{
    public class RetailerController : Controller
    {
        public readonly EmployeeSupport _employeesupport;
        public readonly S3Service _s3support;
        public int UserId;
        private readonly IConfiguration _config;

        public RetailerController(EmployeeSupport employeeSupport, S3Service s3service, IConfiguration _Config)
        {
            _employeesupport = employeeSupport;
            _s3support = s3service;
            _config = _Config;
        }
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpGet]
        public IActionResult LogOut()
        {
            var access = Response.Cookies.GetType();
            return View();
        }
        [HttpGet]
        public IActionResult Customers()
        {
            var access = Response.Cookies.GetType();
            return View();
        }
        [HttpGet]
        public IActionResult Transactions()
        {
            ViewBag.UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            return View();
        }

        [HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerModel model)
        {
            int UserId = Convert.ToInt32(Request.Cookies["UserId"]);

           
            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                TempData["SuccessMessage"] = "First Name is mandatory";
                return RedirectToAction("Customers", "Employee");
            }

            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                TempData["SuccessMessage"] = "Last Name is mandatory";
                return RedirectToAction("Customers", "Employee");
            }

            if (string.IsNullOrWhiteSpace(model.PhoneNumber) || model.PhoneNumber.Length != 10)
            {
                TempData["SuccessMessage"] = "Valid 10 digit Phone Number is mandatory";
                return RedirectToAction("Customers", "Employee");
            }

            if (string.IsNullOrWhiteSpace(model.aadharnumber))
            {
                TempData["SuccessMessage"] = "Valid Aadhaar Number is mandatory";
                return RedirectToAction("Customers", "Employee");
            }

            if (string.IsNullOrWhiteSpace(model.pannumber))
            {
                TempData["SuccessMessage"] = "PAN Number is mandatory";
                return RedirectToAction("Customers", "Employee");
            }
            string folderName = _config["AWS:EmployeeImagePath"]; // will automatically use folder from config

            if (model.AadhaarFrontImage != null && model.AadhaarFrontImage.Length > 0)
            {
                var uploadResult = await _s3support.UploadFileToS3Bucket(model.AadhaarFrontImage);
                model.aadharfrontpath = uploadResult.FileCompletePath;
            }

            if (model.AadhaarBackImage != null && model.AadhaarBackImage.Length > 0)
            {
                var uploadResult = await _s3support.UploadFileToS3Bucket(model.AadhaarBackImage);
                model.aadharbackpath = uploadResult.FileCompletePath;
            }

            if (model.PanFrontImage != null && model.PanFrontImage.Length > 0)
            {
                var uploadResult = await _s3support.UploadFileToS3Bucket(model.PanFrontImage);
                model.panfrontpath = uploadResult.FileCompletePath;
            }

            if (model.PanBackImage != null && model.PanBackImage.Length > 0)
            {
                var uploadResult = await _s3support.UploadFileToS3Bucket(model.PanBackImage);
                model.panbackpath = uploadResult.FileCompletePath;
            }


            var result = _employeesupport.UpdateorInsertCustomer(model, UserId);

            return Json(new
            {
                updatedRowsCount = result.result,
                message = result.StatusMessage
            });

        }

        [HttpGet]
        public IActionResult CustomerDetails(int CustomerId)
        {
            var Model = _employeesupport.GetCustomerDetails(CustomerId);
            if (Model == null || Model.CustomerId == 0)
                return RedirectToAction("Customers");
            return View(Model);
        }

        [HttpGet]
        public IActionResult GetPlatformsByUserId()
        {
            int UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            var result = _employeesupport.GetPlatformsByUserId(UserId);
            return Json(result);
        }
        [HttpGet]
        public IActionResult GetGatewaysByUserId(int platformId)
        {
            if (platformId <= 0)
                return Json(new List<object>());
            int UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            var result = _employeesupport.GetGatewaysByUserId(platformId , UserId);
            return Json(result);
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            int userId = 0;
            if (Request.Cookies["UserId"] != null)
                int.TryParse(Request.Cookies["UserId"], out userId);

            // If no valid UserId, force logout
            if (userId <= 0)
            {
                // If AJAX request, return 401 Unauthorized
                if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Result = new JsonResult(new { error = "Unauthorized" })
                    {
                        StatusCode = 401
                    };
                }
                else
                {
                    // Regular request: redirect to logout
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }
                return;
            }

            // If user is valid, set ViewBag for regular pages
            var user = _employeesupport.GetUserDetails(userId);
            if (user != null)
            {
                ViewBag.EmployeeName = user.UserName;
            }
        }

    }
}
