using Microsoft.AspNetCore.Http;
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
        public List<FileItem> SourceFiles { get; set; }

        [BindProperty]
        public List<FileItem> TargetFiles { get; set; }

        public FileCompares FileCompares { get; set; }


        public void OnGet()
        {
            SourceFiles = _fileProvider.GetDirectoryContents(string.Empty).Select(x => new FileItem {
                FileLength = x.Length,
                FileName = x.Name,
                FilePath = x.PhysicalPath,
                IsSelected = false
            }).ToList();

            TargetFiles = new List<FileItem>(SourceFiles);
        }

        public IActionResult OnPostCompare(IEnumerable<FileItem> SourceFiles, IEnumerable<FileItem> TargetFiles)
        {
            var req = Request;
            OnGet();
            //CurrentFile = PhysicalFiles.FirstOrDefault();
            return Page();
        }
    }
}