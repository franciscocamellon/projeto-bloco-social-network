using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Web.Controllers
{
    public class UserPhotosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserPhotosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserPhotos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Photos.ToListAsync());
        }

        // GET: UserPhotos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPhotos = await _context.Photos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPhotos == null)
            {
                return NotFound();
            }

            return View(userPhotos);
        }

        // GET: UserPhotos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserPhotos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserPhoto,AddingDate")] UserPhotos userPhotos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userPhotos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userPhotos);
        }

        // GET: UserPhotos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPhotos = await _context.Photos.FindAsync(id);
            if (userPhotos == null)
            {
                return NotFound();
            }
            return View(userPhotos);
        }

        // POST: UserPhotos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserPhoto,AddingDate")] UserPhotos userPhotos)
        {
            if (id != userPhotos.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userPhotos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserPhotosExists(userPhotos.Id))
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
            return View(userPhotos);
        }

        // GET: UserPhotos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPhotos = await _context.Photos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPhotos == null)
            {
                return NotFound();
            }

            return View(userPhotos);
        }

        // POST: UserPhotos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userPhotos = await _context.Photos.FindAsync(id);
            _context.Photos.Remove(userPhotos);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserPhotosExists(int id)
        {
            return _context.Photos.Any(e => e.Id == id);
        }
    }
}
