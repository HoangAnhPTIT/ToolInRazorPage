using Microsoft.AspNetCore.Identity;
using SampleApp.Data.Entities;
using SampleApp.Models;
using System.Threading.Tasks;

namespace SampleApp.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<object> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            var user = await _userManager.FindByNameAsync(changePasswordRequest.UserName);
            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var identityResult = await _userManager.ResetPasswordAsync(user, resetPasswordToken, changePasswordRequest.NewPassword);

            return identityResult;
        }
    }
}