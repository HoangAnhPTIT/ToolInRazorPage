using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SampleApp.Data.Entities;
using SampleApp.Models;
using SkiaSharp;
using System;
using System.Threading.Tasks;

namespace SampleApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _env = env;
            _userManager = userManager;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterModel model)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (createResult.Succeeded)
                {
                    System.IO.Directory.CreateDirectory(_env.WebRootPath + "/" + model.UserName);
                }

                return Ok(new
                {
                    result = createResult
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}