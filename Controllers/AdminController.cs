using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using vnenterprises.Filters;
using vnenterprises.Models;
using vnenterprises.Support;
namespace vnenterprises.Controllers
{
    public class AdminController : Controller
    {
        public readonly AdminSupport _adminsupport;
        public readonly EmployeeSupport _employeesupport;
        public readonly S3Service _s3service;
        public int UserId;

        public AdminController(AdminSupport adminsupport, S3Service s3service , EmployeeSupport employeesupport)
        {
            _adminsupport = adminsupport;
            _s3service = s3service;
            _employeesupport = employeesupport;
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult Index()
        {
            UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            return View();
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult Customers()
        {
            UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            return View();
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult Employee()
        {
            return View();
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult Franchise()
        {
            return View();
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult ManageFranchise()
        {

            return View();
        }
        
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult PaymentGateway()
        {
            return View();
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult Manager()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Retailer()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddRetailer()
        {
            PlatformGatewayViewModel platforms = _adminsupport.GetPlatformGatewayList();
            List<Branches> branches = _adminsupport.GetBranchList();

            EmployeeModel model = new EmployeeModel
            {
                PlatformGatewayModel = platforms,
                branchmodel = branches
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult AddEmployee()
        {
            PlatformGatewayViewModel platforms = _adminsupport.GetPlatformGatewayList();
            List<Branches> branches = _adminsupport.GetBranchList();

            EmployeeModel model = new EmployeeModel
            {
                PlatformGatewayModel = platforms,
                branchmodel = branches
            };
            return View(model);
        }
        public IActionResult GetBranches()
        {
            var result = _adminsupport.GetBranchList();
            return Json(result);
        }
        public IActionResult GetTransactionSummary()
        {
            var result = _adminsupport.GetTransactionSummary();
            return Json(result);
        }

        public IActionResult GetTransaction([FromBody] TransactionviewModel model)
        {
            var result = _adminsupport.GetTransactionsForAdmin(model);
            return Json(new
            {
                data = result.data,
                totalCount = result.totalCount
            });
        }

        public IActionResult GetTransactionForExcel([FromBody] TransactionviewModel model)
        {
            var result = _adminsupport.GetTransactionsForAdmin(model);
            return Json(new
            {
                data = result.data,
                totalCount = result.totalCount
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
        public IActionResult GetUserRoles()
        {
            var result = _adminsupport.GetUserRoles();
            return Json(result);
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult EditEmployee(GetEmployeeModel model)
        {
            var result = _adminsupport.getEditEmployeeDetail(model);
            PlatformGatewayViewModel platforms = _adminsupport.GetPlatformGatewayList();
            List<Branches> branches = _adminsupport.GetBranchList();

            result.PlatformGatewayModel = platforms;
            result.branchmodel = branches;
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(EmployeeModel modelobj)
        {
            modelobj.Password = ConvertToBase64(modelobj.Password);
            //modelobj.MPIN = ConvertToBase64(modelobj.MPIN);
            UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            if (modelobj.AadhaarFrontImage != null && modelobj.AadhaarFrontImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.AadhaarFrontImage);
                modelobj.aadharfrontpath = uploadResult.FileCompletePath;
            }
            if (modelobj.AadhaarBackImage != null && modelobj.AadhaarBackImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.AadhaarBackImage);
                modelobj.aadharbackpath = uploadResult.FileCompletePath;
            }
            if (modelobj.PanFrontImage != null && modelobj.PanFrontImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.PanFrontImage);
                modelobj.panfrontpath = uploadResult.FileCompletePath;
            }
            if (modelobj.PanBackImage != null && modelobj.PanBackImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.PanBackImage);
                modelobj.panbackpath = uploadResult.FileCompletePath;
            }
            var response = _adminsupport.UpdateorInsertEmployee(modelobj, UserId);

            return Json(new
            {
                success = response.result == 1,  // adjust if needed
                message = response.StatusMessage
            });
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult AddCustomer()
        {
            return View();
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult CustomerKyc()
        {
            return View();
        }

        public IActionResult GetCustomers(GetEmployeeModel model)
        {
            if (model == null)
                return Json(new List<object>());
            var result = _adminsupport.GetCustomers(model);
            return Json(new
            {
                data = result.data,
                totalCount = result.totalCount
            });
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult EditCustomer()
        {
            return View();
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult Transactions()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetEmployees(string branchIds, string roleIds , string kyc , string search , int page , int pageSize)
        {
            var getEmployeeModel = new GetEmployeeModel
            {
                BranchIds = branchIds,
                RoleIds = roleIds,
                Kyc = kyc,
                SearchText = search ?? "",
                page = page,
                pageSize = pageSize
            };
            var result = _adminsupport.getEmployeeDetail(getEmployeeModel);
            return Json(new
            {
                data = result.data,
                totalCount = result.totalCount
            });
        }

        public IActionResult GetKycCounts()
        {
            var result = _adminsupport.GetKycCounts(1);
            return Json(result);
        }
        public IActionResult GetCustomerKycCounts()
        {
            var result = _adminsupport.GetKycCounts(2);
            return Json(result);
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult AddManager()
        {
            //PlatformGatewayViewModel model = _adminsupport.GetPlatformGatewayList();
            List<Branches> branches = _adminsupport.GetBranchList();

            ManagerModel manmodel = new ManagerModel
            {
                //platformgatwaymodel = model,
                branchmodel = branches
            };

            return View(manmodel);
        }

        [HttpPost]
        [AuthorizeUser(1)]
        public IActionResult AddManager(ManagerModel model)
        {
            model.Password = ConvertToBase64(model.Password);
            //model.MPIN = ConvertToBase64(model.MPIN);
            UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            var response = _adminsupport.UpdateorInsertManager(model , UserId);

            return Json(new
            {
                success = response.result == 1,  // adjust if needed
                message = response.StatusMessage
            });


        }
        [HttpPost]
        [AuthorizeUser(1)]
        public IActionResult EditManager(ManagerModel model)
        {
            model.Password = ConvertToBase64(model.Password);
            //model.MPIN = ConvertToBase64(model.MPIN);
            UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            var response = _adminsupport.UpdateorInsertManager(model, UserId);

            return Json(new
            {
                success = response.result == 1,  // adjust if needed
                message = response.StatusMessage
            });


        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult EditManager(int UserId)
        {
            ManagerModel model = new ManagerModel();
            var result = _adminsupport.GetManagerDetails(UserId);
            List<Branches> branches = _adminsupport.GetBranchList();
            result.branchmodel = branches;
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromForm] EmployeeModel modelobj)
        {
            modelobj.Password = ConvertToBase64(modelobj.Password);
            //modelobj.MPIN = ConvertToBase64(modelobj.MPIN);
            UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            if(modelobj.AadhaarFrontImage != null && modelobj.AadhaarFrontImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.AadhaarFrontImage);
                modelobj.aadharfrontpath = uploadResult.FileCompletePath;
            }
            if (modelobj.AadhaarBackImage != null && modelobj.AadhaarBackImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.AadhaarBackImage);
                modelobj.aadharbackpath = uploadResult.FileCompletePath;
            }
            if (modelobj.PanFrontImage != null && modelobj.PanFrontImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.PanFrontImage);
                modelobj.panfrontpath = uploadResult.FileCompletePath;
            }
            if (modelobj.PanBackImage != null && modelobj.PanBackImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.PanBackImage);
                modelobj.panbackpath = uploadResult.FileCompletePath;
            }
            var response = _adminsupport.UpdateorInsertEmployee(modelobj, UserId);

            return Json(new
            {
                success = response.result == 1,  // adjust if needed
                message = response.StatusMessage
            });

        }
        [HttpPost]
        public async Task<IActionResult> AddRetailer([FromForm] EmployeeModel modelobj)
        {
            modelobj.Password = ConvertToBase64(modelobj.Password);
            //modelobj.MPIN = ConvertToBase64(modelobj.MPIN);
            UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            if (modelobj.AadhaarFrontImage != null && modelobj.AadhaarFrontImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.AadhaarFrontImage);
                modelobj.aadharfrontpath = uploadResult.FileCompletePath;
            }
            if (modelobj.AadhaarBackImage != null && modelobj.AadhaarBackImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.AadhaarBackImage);
                modelobj.aadharbackpath = uploadResult.FileCompletePath;
            }
            if (modelobj.PanFrontImage != null && modelobj.PanFrontImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.PanFrontImage);
                modelobj.panfrontpath = uploadResult.FileCompletePath;
            }
            if (modelobj.PanBackImage != null && modelobj.PanBackImage.Length > 0)
            {
                var uploadResult = await _s3service.UploadFileToS3Bucket(modelobj.PanBackImage);
                modelobj.panbackpath = uploadResult.FileCompletePath;
            }
            var response = _adminsupport.UpdateorInsertRetailer(modelobj, UserId);

            return Json(new
            {
                success = response.result == 1,  // adjust if needed
                message = response.StatusMessage
            });

        }
        public string ConvertToBase64(string plainPassword)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return Convert.ToBase64String(plainTextBytes);
        }

        [HttpPost]
        [HttpPost]
        [AuthorizeUser(1)]
        public IActionResult SavePlatform([FromBody] Platforms model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name))
                return BadRequest("Invalid data");

            try
            {
                var savedPlatform = _adminsupport.SaveOrUpdatePlatform(model);
                return Json(new { success = true, platform = savedPlatform });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [AuthorizeUser(1)]
        public IActionResult SaveGateway([FromBody] Gateway model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name) || model.PlatformId <= 0)
                return BadRequest("Invalid gateway data");

            try
            {
                var savedGateway = _adminsupport.SaveOrUpdateGateway(model);
                return Json(new { success = true, gateway = savedGateway });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult Platform()
        {
            UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            PlatformGatewayViewModel model = _adminsupport.GetPlatformGatewayList();

            return View(model);
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
