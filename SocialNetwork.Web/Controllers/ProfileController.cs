using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Domain.Entities;
using System.Threading.Tasks;

namespace SocialNetwork.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ProfileController(UserManager<User> userManager,
                                 SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //action para exibição da view
        public async Task<IActionResult> Edit()
        {
            var user = await GetUserAsync();
            return View(user);
        }

        //action para atualização dos dados
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] string firstName, string lastName)
        {
            //recuperando user completo do banco de dados
            var userFromDb = await GetUserAsync();

            //recuperando dados do formulário (view)
            userFromDb.FirstName = firstName;
            userFromDb.LastName = lastName;

            //atualizando dados
            await _userManager.UpdateAsync(userFromDb);

            //atualizando dados da sessão atual
            await _signInManager.RefreshSignInAsync(userFromDb);

            ViewBag.ShowMessage = true;
            return View(userFromDb);
        }

        private async Task<User> GetUserAsync() => await _userManager.GetUserAsync(User);

    }
}
