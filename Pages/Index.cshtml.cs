using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using SampleApp.Data;
using SampleApp.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace SampleApp.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;
        public IndexModel(AppDbContext context, IFileProvider fileProvider, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment env)
        {
            _context = context;
            _signInManager = signInManager;
            _env = env;
        }

        //public IList<AppFile> DatabaseFiles { get; private set; }
        public List<IFileInfo> PhysicalFiles { get; private set; }

        public async Task OnGetAsync()
        {
            IFileProvider _fileProvider = new PhysicalFileProvider(_env.WebRootPath + "/" + User.Identity.Name);
            PhysicalFiles = _fileProvider.GetDirectoryContents(string.Empty).Where(x => x.Name.Contains("xlsx")).ToList();
        }

        public IActionResult OnGetDownloadPhysical(string fileName)
        {
            IFileProvider _fileProvider = new PhysicalFileProvider(_env.WebRootPath + "/" + User.Identity.Name);

            var downloadFile = _fileProvider.GetFileInfo(fileName);

            return PhysicalFile(downloadFile.PhysicalPath, MediaTypeNames.Application.Octet, fileName);
        }
    }
}