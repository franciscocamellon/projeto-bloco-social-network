using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Interfaces.Infrastructure;
using System;
using System.Threading.Tasks;

namespace SocialNetwork.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IProfileRepository _profileRepository;

        public ProfileController(UserManager<User> userManager,
                                 IProfileRepository profileRepository)
        {
            _userManager = userManager;
            _profileRepository = profileRepository;
         }

        //action para exibição da view
        public async Task<IActionResult> Edit()
        {
            //recuperando user completo do banco de dados
            var currentUserId = _userManager.GetUserId(User);

            //obter a entidade perfil
            var profile = await _profileRepository.GetProfileByUserIdAsync(currentUserId);

            return View(profile);
        }

        //action para atualização dos dados
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] Profile profileForm)
        {
            //recuperando user completo do banco de dados
            var currentUserId = _userManager.GetUserId(User);

            //obter a entidade perfil
            var profileFromBd = await _profileRepository.GetProfileByUserIdAsync(currentUserId);
            
            //atribuindo o id do user (identity) ao profile
            profileForm.UserId = currentUserId;

            //validando se usuário existe em banco, e se existir, valida id do formulário com id do banco
            if (profileFromBd == null)
            {
                await _profileRepository.InsertAsync(profileForm);

                return RedirectToAction(nameof(Edit));
            }
            else
            {
                if (profileFromBd.Id != profileForm.Id)
                    throw new ApplicationException("UserId is different from database");

                //atualizando a entidade perfil
                await _profileRepository.UpdateAsync(profileForm);
            }

            ViewBag.ShowMessage = true;
            return View(profileForm);
        }
    }
}
