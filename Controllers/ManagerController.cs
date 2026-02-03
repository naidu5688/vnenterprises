using Microsoft.AspNetCore.Mvc;

namespace vnenterprises.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
