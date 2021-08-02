using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Interfaces.Respositories;
using System.Threading.Tasks;

namespace SocialNetwork.Web.Controllers
{
    public class FeedController : Controller
    {
        private readonly IPostRepository _postRepository;
        public FeedController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var posts = await _postRepository.GetLastPostsAsync();

            return View(posts);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> OnInsertPost([FromForm] Post post)
        {
            await _postRepository.InsertAsync(post);

            return RedirectToAction(nameof(Index));
        }
    }
}
