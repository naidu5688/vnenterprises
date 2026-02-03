using Microsoft.AspNetCore.Mvc;

namespace vnenterprises.Controllers
{
    public class RetailerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
