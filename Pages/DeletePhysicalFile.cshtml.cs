using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;

namespace SampleApp.Pages
{
    public class DeletePhysicalFileModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public DeletePhysicalFileModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IFileInfo RemoveFile { get; private set; }

        public IActionResult OnGet(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return RedirectToPage("/Index");
            }
            IFileProvider _fileProvider = new PhysicalFileProvider(_env.WebRootPath + "/" + User.Identity.Name);

            RemoveFile = _fileProvider.GetFileInfo(fileName);

            if (!RemoveFile.Exists)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public IActionResult OnPost(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return RedirectToPage("/Index");
            }

            IFileProvider _fileProvider = new PhysicalFileProvider(_env.WebRootPath + "/" + User.Identity.Name);
            RemoveFile = _fileProvider.GetFileInfo(fileName);

            if (RemoveFile.Exists)
            {
                System.IO.File.Delete(RemoveFile.PhysicalPath);
            }

            return RedirectToPage("./Index");
        }
    }
}
