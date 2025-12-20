using Microsoft.AspNetCore.Mvc;

namespace api_joyeria.Api.Controllers
{
    public class WebhookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
