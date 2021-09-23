using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SocialNetwork.Data;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Interfaces.Repositories;
using SocialNetwork.Web.Models;

namespace SocialNetwork.Web.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IProfileRepository _profileRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IPictureRepository _pictureRepository;

        public AlbumsController(ApplicationDbContext context,
                                UserManager<User> userManager,
                                IProfileRepository profileRepository,
                                IAlbumRepository albumRepository,
                                IPictureRepository pictureRepository)
        {
            _context = context;
            _userManager = userManager;
            _profileRepository = profileRepository;
            _albumRepository = albumRepository;
            _pictureRepository = pictureRepository;
        }

        // GET: Albums
        public async Task<IActionResult> Index(AlbumIndexViewModel albumIndexViewRequest)
        {
            var albumIndexViewModel = new AlbumIndexViewModel
            {
                Albums = await _albumRepository.GetAllAsync()
            };
            var currentUserId = _userManager.GetUserId(User);
            var profile = await _profileRepository.GetProfileByUserIdAsync(currentUserId);
            

            var albumsByProfile = await _albumRepository.GetAlbumsByProfileIdAsync(profile.Id);
            //var albumsByProfile = profile.Albums;

            return View(albumIndexViewModel);
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chosenAlbum = await _albumRepository.GetByIdAsync(id.Value);
            
            if (chosenAlbum == null)
            {
                return NotFound();
            }

            return View(chosenAlbum);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            return View();
        }
        
        // GET: Albums/Create
        public IActionResult CreatePicture()
        {
            return View();
        }

        // POST: Albums/CreatePicture
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreatePicture(IFormCollection form,
        //                                               [FromServices] IHttpClientFactory clientFactory,
        //                                               Picture picture,
        //                                               Album album)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (var content = new MultipartFormDataContent())
        //        {
        //            foreach (var file in form.Files)
        //            {
        //                content.Add(CreateFileContent(file.OpenReadStream(), file.FileName, file.ContentType));
        //            }

        //            var httpClient = clientFactory.CreateClient();
        //            var response = await httpClient.PostAsync("api/image", content);

        //            response.EnsureSuccessStatusCode();
        //            var responseResult = await response.Content.ReadAsStringAsync();
        //            var uriImage = JsonConvert.DeserializeObject<string[]>(responseResult).FirstOrDefault();
                
        //            //recuperando user completo do banco de dados
        //            var currentUserId = _userManager.GetUserId(User);

        //            //obter a entidade perfil do banco
        //            var profileFromBd = await _profileRepository.GetProfileByUserIdAsync(currentUserId);

        //            if (profileFromBd == null)
        //            {
        //                var albumToInsert = new Album();
        //                albumToInsert.Id = Guid.NewGuid();
        //                albumToInsert.
        //            }

        //            var chosenAlbum = await _albumRepository.GetByIdAsync(id.Value);
        //            profileFromBd.
        //            album.ProfileId = profile.Id;
        //            picture.Id = Guid.NewGuid();
        //            picture.AlbumId = album.Id;
        //            picture.UploadDate = DateTime.Now;
        //            picture.UriImageAlbum = uriImage;

        //            await _pictureRepository.CreateAsync(picture);

        //            return RedirectToAction(nameof(Index));
        //        }
        //    }
        //    return View(picture);
        //}

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreationDate,AlbumName,ProfileId")] Album album)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = _userManager.GetUserId(User);
                var profile = await _profileRepository.GetProfileByUserIdAsync(currentUserId);

                album.ProfileId = profile.Id;
                album.Id = Guid.NewGuid();

                await _albumRepository.CreateAsync(album);

                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chosenAlbum = await _albumRepository.GetByIdAsync(id.Value);
            if (chosenAlbum == null)
            {
                return NotFound();
            }
            return View(chosenAlbum);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CreationDate,AlbumName,ProfileId")] Album album)
        {
            if (id != album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _albumRepository.EditAsync(album);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var albumExist = await AlbumExists(album.Id);
                    if (albumExist)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chosenAlbum = await _albumRepository.GetByIdAsync(id.Value);

            if (chosenAlbum == null)
            {
                return NotFound();
            }

            return View(chosenAlbum);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _albumRepository.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AlbumExists(Guid id)
        {
            var album = await _albumRepository.GetByIdAsync(id);

            var any = album != null;

            return any;
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
    }
}
