using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialNetwork.Domain.Entities;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

    }
}
