using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SampleApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SampleApp.Pages
{
    [BindProperties]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Required(ErrorMessage ="Đây là trường bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Đây là trường bắt buộc")]
        public string Password { get; set; }

        public bool RememberLogin { get; set; }

        public List<string> ErrorMessages { get; set; } = new List<string>();

        public async Task OnGet()
        {
           
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result = await _signInManager.PasswordSignInAsync(Username, Password, RememberLogin, false); 

            if(result.Succeeded)
            {
                return Redirect("Index");
            }
            else
            {
                ErrorMessages.Add("Tên đăng nhập hoặc mật khẩu không chính xác");
                return Page();
            }
        }
    }
}
