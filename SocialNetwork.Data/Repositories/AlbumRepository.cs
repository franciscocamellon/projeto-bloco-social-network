using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Interfaces.Repositories;

namespace SocialNetwork.Data.Repositories
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AlbumRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AlbumModel>> GetAlbumsByProfileIdAsync(int profileId)
        {
            //var albums = _dbContext.Albums.Where(x => x.ProfileId == profileId);
            var albums = _dbContext.Albums
                .Include(z => z.Pictures);

            return await albums.ToListAsync();
        }

        public async Task<IEnumerable<AlbumModel>> GetAllAsync()
        {
            var albums = _dbContext.Albums.AsQueryable();
            var pictures = _dbContext.Pictures.AsQueryable();

            var result = await albums
                .Select(x => new
                {
                    Albuns = x,
                    Pictures = pictures.Where(z => z.AlbumId == x.Id).ToList()
                })
                .ToListAsync();

            var albumResult = result
                .Select(x =>
                {
                    x.Albuns.Pictures = x.Pictures;
                    return x.Albuns;
                });

            return albumResult;
        }
        public async Task<AlbumModel> GetByIdAsync(Guid id)
        {
            var album = await _dbContext.Albums
                             .Include(z => z.Pictures)
                             .FirstOrDefaultAsync(x => x.Id == id);

            return album;
        }
        public async Task<AlbumModel> CreateAsync(AlbumModel albumModel)
        {
            var createdAlbum = _dbContext.Albums.Add(albumModel);

            await _dbContext.SaveChangesAsync();

            return createdAlbum.Entity;
        }
        public async Task<AlbumModel> EditAsync(AlbumModel album)
        {
            var updatedAlbum = _dbContext.Albums.Update(album);
            await _dbContext.SaveChangesAsync();

            return updatedAlbum.Entity;
        }
        public async Task DeleteAsync(Guid id)
        {
            var album = await GetByIdAsync(id);

            _dbContext.Albums.Remove(album);

            await _dbContext.SaveChangesAsync();
        }
    }
}
