using JobPortal.Features.User;
using JobPortal.Features.User.Interface;
using JobPortal.Shared.Interfaces;

namespace JobPortal.Features.User.Interface
{
    public interface IUserRepository : IRepository<User1>
    {
        Task<int> RegisterUserAsync(string fullName, string email, string username,
                                    string passwordHash, string salt,
                                    string? mobileNumber = null, string role = "jobseeker");

        Task<User1?> LoginUserAsync(string emailOrMobile, string passwordHash);

        Task<User1?> GetByEmailAsync(string email);
        Task<User1?> GetByUsernameAsync(string username);
        Task<IEnumerable<User1>> SearchAsync(string keyword);

        Task<bool> SetActiveAsync(int userId, bool isActive);
        Task<bool> SetEmailVerifiedAsync(int userId, bool verified);
        Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash, string newSalt);
    }
}
