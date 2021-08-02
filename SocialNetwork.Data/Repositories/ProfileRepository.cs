using Microsoft.EntityFrameworkCore;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Data.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        public ApplicationDbContext _dbContext { get; set; }

        public ProfileRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Profile> GetProfileByUserIdAsync(string userId)
        {
            return await _dbContext.Profiles
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task InsertAsync(Profile profile)
        {
            await _dbContext.Profiles.AddAsync(profile);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Profile profile)
        {
            _dbContext.Update(profile);
            await _dbContext.SaveChangesAsync();
        }
    }
}
