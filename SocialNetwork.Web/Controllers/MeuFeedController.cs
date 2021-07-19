using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Web.Controllers
{
    public class MeuFeedController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
