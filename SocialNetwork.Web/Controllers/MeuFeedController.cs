using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Web.Controllers
{
    public class MeuFeedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
