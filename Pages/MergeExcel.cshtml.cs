using Aspose.Cells;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using SampleApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Range = Aspose.Cells.Range;

namespace SampleApp.Pages
{
    public class MergeExcelModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public MergeExcelModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        [BindProperty]
        public List<FileItem> SourceFiles { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 40;

        public int TotalPages { get; set; }

        public List<string> ErrorMessage { get; set; } = new List<string>();

        public void OnGet()
        {
            IFileProvider _fileProvider = new PhysicalFileProvider(_env.WebRootPath + "/" + User.Identity.Name);
            SourceFiles = _fileProvider.GetDirectoryContents(string.Empty).Where(x => x.Name.Contains("xlsx")).Select(x => new FileItem
            {
                FileLength = x.Length,
                FileName = x.Name,
                FilePath = x.PhysicalPath,
                IsSelected = false
            }).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            Count = _fileProvider.GetDirectoryContents(string.Empty).Where(x => x.Name.Contains("xlsx")).Select(x => new FileItem
            {
                FileLength = x.Length,
                FileName = x.Name,
                FilePath = x.PhysicalPath,
                IsSelected = false
            }).Count();

            TotalPages = Count / PageSize;
        }

        public IActionResult OnPostHandler(IEnumerable<FileItem> SourceFiles)
        {
            try
            {
                SourceFiles = SourceFiles.Where(x => x.IsSelected).ToList();

                if (!SourceFiles.Any())
                {
                    ModelState.AddModelError("Source file not selected", "Chưa chọn file");
                }

                if (!ModelState.IsValid)
                {
                    ErrorMessage = ModelState.SelectMany(x => x.Value.Errors).Select(e => e.ErrorMessage).ToList();
                    return Page();
                }

                var files = SourceFiles.Select(x => x.FilePath).ToArray();

                // Create a cachedFile for the process
                string cacheFile = "test.txt";

                // Output File to be created
                string dest = "output.xlsx";

                // Merge the files in the output file. Supports only .xls files
                CellsHelper.MergeFiles(files, cacheFile, dest);

                // Now if you need to rename your sheets, you may load the output file
                using Workbook workbook = new Workbook("output.xlsx");

                int ix = 1;

                // Browse all the sheets to rename them accordingly
                foreach (Worksheet sheet in workbook.Worksheets)
                {
                    sheet.Name = "S" + ix.ToString();
                    ix++;
                }

                using Workbook destWorkbook = new Workbook();

                Worksheet destSheet = destWorkbook.Worksheets[0];

                int TotalRowCount = 0;

                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {
                    try
                    {
                        Worksheet sourceSheet = workbook.Worksheets[i];

                        Cells cells = sourceSheet.Cells;
                        Cell cell = cells["A5"];
                        var cellData = cell.Value;
                        if (cellData != null && cellData.ToString() == "Evaluation Only. Created with Aspose.Cells for .NET.Copyright 2003 - 2023 Aspose Pty Ltd.")
                        {
                            continue;
                        }

                        Range sourceRange = sourceSheet.Cells.MaxDisplayRange;

                        Range destRange = destSheet.Cells.CreateRange(sourceRange.FirstRow + TotalRowCount, sourceRange.FirstColumn,
                              sourceRange.RowCount, sourceRange.ColumnCount);

                        destRange.Copy(sourceRange);

                        TotalRowCount = sourceRange.RowCount + TotalRowCount;
                    }
                    catch (Exception ex)
                    {
                    }
                }

                using var stream = new MemoryStream();
                destWorkbook.Save(stream, SaveFormat.Xlsx);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8", "final-file.xlsx");
            }
            catch (Exception ex)
            {
                ErrorMessage.Add(ex.Message);

                return Page();
            }
        }
    }
}