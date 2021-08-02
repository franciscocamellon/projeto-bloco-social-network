using SocialNetwork.Domain.Entities;
using System.Threading.Tasks;

namespace SocialNetwork.Domain.Interfaces.Infrastructure
{
    public interface IProfileRepository
    {
        Task<Profile> GetProfileByUserIdAsync(string userId);
        Task InsertAsync(Profile profile);
        Task UpdateAsync(Profile profile);
    }
}
