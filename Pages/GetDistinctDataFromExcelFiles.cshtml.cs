using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using SampleApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.Pages
{
    public class GetDistinctDataFromExcelFilesModel : PageModel
    {
        private readonly IFileProvider _fileProvider;

        public GetDistinctDataFromExcelFilesModel(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        [BindProperty]
        public IEnumerable<FileItem> PhysicalFiles { get; set; }

        public IFileInfo CurrentFile { get; set; }

        [BindProperty]
        public FileCompares FileCompares { get; set; }


        public void OnGet()
        {
            PhysicalFiles = _fileProvider.GetDirectoryContents(string.Empty).Select(x => new FileItem {
                FileLength = x.Length,
                FileName = x.Name,
                FilePath = x.PhysicalPath,
                IsSelected = true
            });
        }

        public IActionResult OnPostCompare(IEnumerable<FileItem> fileItems)
        {
            OnGet();
            //CurrentFile = PhysicalFiles.FirstOrDefault();
            return Page();
        }
    }
}