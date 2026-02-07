using Amazon.S3.Model.Internal.MarshallTransformations;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Threading.Tasks;
using vnenterprises.Filters;
using vnenterprises.Models;
using vnenterprises.Support;

namespace vnenterprises.Controllers
{
    public class EmployeeController : Controller
    {
        public readonly EmployeeSupport _employeesupport;
        public readonly S3Service _s3support;
        public int UserId;
        private readonly IConfiguration _config;
        public EmployeeController(EmployeeSupport employeeSupport , S3Service s3service , IConfiguration _Config)
        {
            _employeesupport = employeeSupport;
            _s3support = s3service;
            _config = _Config;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var userId = Request.Cookies["UserId"];
            int loggedUserId = 0;
            if (!string.IsNullOrEmpty(userId))
            {
                loggedUserId = Convert.ToInt32(userId);
            }

            userDetailsReposne result = _employeesupport.GetUserDetails(loggedUserId);
            ViewBag.EmployeeName = result.UserName;
            var access = Response.Cookies.GetType();
            return View(result);
        }

        [HttpGet]
        public IActionResult Transactions()
        {
            ViewBag.UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            return View();
        }

        public IActionResult GetTransactionById(int transactionid)
        {
            if(transactionid == 0)
                return Json(new List<object>());
            TransactionviewModel model = new TransactionviewModel();
            model.TransactionId = transactionid;
            model.TransactionType = "";
            model.SearchText = "";
            model.StartDate = "";
            model.EndDate = "";
            model.PageNo = 1;
            model.PageSize = 1;
            model.UserId = 0;
            var result = _employeesupport.GetTransactions(model);
            return Json(result.data.FirstOrDefault());
        }

        [HttpPost]
        public IActionResult GetTransactions([FromBody] TransactionviewModel model)
        {
            if (model == null)
                return Json(new List<object>());
            var result = _employeesupport.GetTransactions(model);

            return Json(new
            {
                data = result.data,
                totalCount = result.totalCount
            });
        }

        [HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetCardTypes()
        {
            var data = _employeesupport.GetActiveCardTypes();

            return Json(data.Select(x => new
            {
                id = x.Id,
                name = x.Name
            }));
        }
        [HttpGet]
        public IActionResult CustomerDetails(int CustomerId)
        {
            var Model = _employeesupport.GetCustomerDetails(CustomerId);
            if (Model == null || Model.CustomerId == 0)
                return RedirectToAction("Customers");
            return View(Model);
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerModel model)
        {
            int UserId = Convert.ToInt32(Request.Cookies["UserId"]);

            /* ===============================
               1️⃣ CUSTOMER MANDATORY VALIDATION
               =============================== */

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

        [HttpPost]
        public IActionResult AddTransaction([FromBody] TransactionCreateDto model)
        {
            if (model == null)
                return Json(new { result = 0, message = "Invalid data" });
            var userId = Convert.ToInt32(Request.Cookies["UserId"]);
            if (userId == null)
                return Json(new { result = 0, message = "Invalid data" });
            var result = _employeesupport.AddTransaction(model , userId);

            if(result > 0)
                return Json(new { result = result, message = "Transaction Created Successfully [" + result + "]"});
            else
                return Json(new { result = 0, message = "Something went wrong . Please try agian" });
        }

        [HttpGet]
        public IActionResult GetCustomerIdByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return Json(new { customerId = 0 });

            int customerId = _employeesupport.GetCustomerIdByMobile(mobile);

            if (customerId > 0)
            {
                return Json(new
                {
                    success = true,
                    exists = true,
                    customerId = customerId
                });
            }

            return Json(new
            {
                success = true,
                exists = false
            });
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
        public  IActionResult GetPlatforms()
        {
            var result = _employeesupport.GetPlatformlist();
            return Json(result);
        }
        [HttpGet]
        public IActionResult GetGateways(int platformId)
        {
            if (platformId <= 0)
                return Json(new List<object>());  

            var result = _employeesupport.GetGatewaylist(platformId);
            return Json(result);
        }
        [HttpGet]
        public IActionResult GetBanks()
        {
            var result = _employeesupport.GetBankList();
            return Json(result);
        }
        [HttpGet]
        public IActionResult TranasactionTypes()
        {
            var result = _employeesupport.GetTranasactionTypesList();
            return Json(result);
        }

        public IActionResult GetUserTransaction(int userId)
        {
            if (userId == 0)
                return Json(new List<object>());
            var result = _employeesupport.GetUserTransaction(userId);
            return Json(result);
        }
        [HttpGet]
        public IActionResult GetCustomerCards(int customerId)
        {
            var result = _employeesupport.GetCustomerCardsAndBanks(customerId);
            return Json(result);
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (Request.Cookies["UserId"] != null)
            {
                var userId = Convert.ToInt32(Request.Cookies["UserId"]);
                var user = _employeesupport.GetUserDetails(userId);
                if (user != null)
                {
                    ViewBag.EmployeeName = user.UserName;
                }
            }
        }

    }
}
