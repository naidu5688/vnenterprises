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
        public readonly S3Service _s3service;
        public int UserId;

        public AdminController(AdminSupport adminsupport, S3Service s3service)
        {
            _adminsupport = adminsupport;
            _s3service = s3service;
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
        
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult EditEmployee()
        {
            return View();
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult AddCustomer()
        {
            return View();
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
            model.MPIN = ConvertToBase64(model.MPIN);
            UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            var response = _adminsupport.UpdateorInsertManager(model , UserId);

            TempData["SuccessMessage"] = response.StatusMessage.ToString();
            return RedirectToAction("Manager", "Admin");
           
                
        }
        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult EditManager(int ManagerId)
        {
            ManagerModel model = new ManagerModel();
            var result = _adminsupport.GetManagerDetails(ManagerId);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromForm] EmployeeModel modelobj)
        {
            modelobj.Password = ConvertToBase64(modelobj.Password);
            modelobj.MPIN = ConvertToBase64(modelobj.MPIN);
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
            TempData["SuccessMessage"] = response.StatusMessage.ToString();
            return RedirectToAction("Manager", "Admin");
        }


        public string ConvertToBase64(string plainPassword)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return Convert.ToBase64String(plainTextBytes);
        }

        [HttpPost]
        [AuthorizeUser(1)]
        public IActionResult AddPlatform(Platforms platobj)
        {
            if (platobj.Id == 0)
                return View();
            else
            {

            }
                return View();
        }

        [HttpGet]
        [AuthorizeUser(1)]
        public IActionResult Platform()
        {
            UserId = Convert.ToInt32(Request.Cookies["UserId"]);
            PlatformGatewayViewModel model = _adminsupport.GetPlatformGatewayList();

            return View(model);
        }



    }

}
