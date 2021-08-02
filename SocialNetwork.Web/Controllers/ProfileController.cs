using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        [HttpPost]
        public async Task<IActionResult> EditImageProfile(IFormCollection form, 
                                           [FromServices] IHttpClientFactory clientFactory)
        {
            using (var content = new MultipartFormDataContent())
            {
                foreach (var file in form.Files)
                {
                    content.Add(CreateFileContent(file.OpenReadStream(), file.FileName, file.ContentType));
                }
                
                var httpClient = clientFactory.CreateClient();
                var response = await httpClient.PostAsync("api/image", content);
                
                response.EnsureSuccessStatusCode();
                var responseResult = await response.Content.ReadAsStringAsync();
                var uriImagem = JsonConvert.DeserializeObject<string[]>(responseResult).FirstOrDefault();


                //recuperando user completo do banco de dados
                var userFromDb = await GetUserAsync();

                //recuperando dados do formulário (view)
                userFromDb.UriImageProfile = uriImagem;

                //atualizando dados
                await _userManager.UpdateAsync(userFromDb);

                //atualizando dados da sessão atual
                await _signInManager.RefreshSignInAsync(userFromDb);
            }

            ViewBag.ShowMessage = true;
            return RedirectToAction(nameof(Edit));
        }

        private StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"files\"",
                FileName = "\"" + fileName + "\""
            };

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return fileContent;
        }

        private async Task<User> GetUserAsync() => await _userManager.GetUserAsync(User);

            ViewBag.ShowMessage = true;
            return View(profileForm);
        }
    }
}
