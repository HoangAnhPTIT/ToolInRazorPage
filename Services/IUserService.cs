using SampleApp.Models;
using System.Threading.Tasks;

namespace SampleApp.Services
{
    public interface IUserService
    {
        Task<object> ChangePassword(ChangePasswordRequest changePasswordRequest);
    }
}
