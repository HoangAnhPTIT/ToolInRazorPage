using Aspose.Cells;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using SampleApp.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        public bool IsCompareProcessing { get; set; }

        public IEnumerable<string> ErrorMessage { get; set; }


        public void OnGet()
        {
            SourceFiles = _fileProvider.GetDirectoryContents(string.Empty).Select(x => new FileItem
            {
                FileLength = x.Length,
                FileName = x.Name,
                FilePath = x.PhysicalPath,
                IsSelected = false
            }).ToList();

            TargetFiles = new List<FileItem>(SourceFiles);
        }

        public async Task<IActionResult> OnPostCompare(IEnumerable<FileItem> SourceFiles, IEnumerable<FileItem> TargetFiles)
        {
            IsCompareProcessing = true;
            SourceFiles = SourceFiles.Where(x => x.IsSelected).ToList();
            TargetFiles = TargetFiles.Where(x => x.IsSelected).ToList();

            if (!SourceFiles.Any())
            {
                ModelState.AddModelError("Source file not selected", "Chưa chọn file gốc");
            }

            if (!TargetFiles.Any())
            {
                ModelState.AddModelError("Target file not selected", "Chưa chọn file so sánh");
            }

            foreach (var file in TargetFiles)
            {
                if (SourceFiles.Any(x => x.FileName == file.FileName)) ModelState.AddModelError("Duplicate File", $"File {file.FileName} đã được chọn");
            }

            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.SelectMany(x => x.Value.Errors).Select(e => e.ErrorMessage);
                IsCompareProcessing = false;
                return Page();
            }

            //Dictionary<string, int> map = new();

            HashSet<string> set = new();
            List<string> lines = new();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            foreach (var file in SourceFiles)
            {
                using (var package = new ExcelPackage(new FileInfo(file.FilePath)))
                {
                    var ws = package.Workbook.Worksheets[0];

                    for (int rw = 1; rw <= ws.Dimension.End.Row; rw++)
                    {
                        var value = ws.Cells[rw, 1].Value;
                        if (value != null && !string.IsNullOrEmpty(value.ToString()))
                        {
                            set.Add(value.ToString());
                        }

                    }
                }
            }

            foreach (var file in TargetFiles)
            {
                using (var package = new ExcelPackage(new FileInfo(file.FilePath)))
                {
                    var ws = package.Workbook.Worksheets[0];

                    for (int rw = 1; rw <= ws.Dimension.End.Row; rw++)
                    {
                        var value = ws.Cells[rw, 1].Value;
                        if (value != null && !string.IsNullOrEmpty(value.ToString()))
                        {

                            if (set.Add(value.ToString()))
                            {
                                lines.Add(value.ToString());
                            }
                        }

                    }
                }
            }

            set.Clear();

            Workbook book = new Workbook();
            Worksheet sheet = book.Worksheets[0];
            sheet.Cells.ImportArray(lines.ToArray(), 0, 0, true);
            //sheet.Cells.ImportArray(duplicate, 0, 1, true);

            using var stream = new MemoryStream();
            book.Save(stream, SaveFormat.Xlsx);
            var content = stream.ToArray();

            IsCompareProcessing = false;
            await stream.DisposeAsync();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8");
            //book.Save("file.xlsx");


            //IsCompareProcessing = false;
            //return Page();
        }
    }
}