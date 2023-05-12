﻿using Aspose.Cells;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using SampleApp.Models;
using SampleApp.Utilities;
using System;
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

        [BindProperty]
        public bool IsCompareProcessing { get; set; }

        public IEnumerable<string> ErrorMessage { get; set; }

        public void OnGet()
        {
            SourceFiles = _fileProvider.GetDirectoryContents(string.Empty).Where(x => x.Name.Contains("xlsx")).Select(x => new FileItem
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
            try
            {
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

                HashSet<string> set = new HashSet<string>();
                List<string> lines = new List<string>();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                foreach (var file in SourceFiles)
                {
                    using var package = new ExcelPackage(new FileInfo(file.FilePath));
                    var ws = package.Workbook.Worksheets[0];

                    for (int rw = 1; rw <= ws.Dimension.End.Row; rw++)
                    {
                        var value = ws.Cells[rw, 1].Value;
                        if (value != null && !string.IsNullOrEmpty(value.ToString()))
                        {
                            var phoneNumber = PhoneNumberUtility.StandardizedPhoneNumber(value);
                            set.Add(phoneNumber.ToString());
                        }
                    }
                }

                foreach (var file in TargetFiles)
                {
                    using var package = new ExcelPackage(new FileInfo(file.FilePath));
                    var ws = package.Workbook.Worksheets[0];

                    for (int rw = 1; rw <= ws.Dimension.End.Row; rw++)
                    {
                        var value = ws.Cells[rw, 1].Value;
                        if (value != null && !string.IsNullOrEmpty(value.ToString()))
                        {
                            var phoneNumber = PhoneNumberUtility.StandardizedPhoneNumber(value);

                            if (set.Add(phoneNumber))
                            {
                                lines.Add(phoneNumber);
                            }
                        }
                    }
                }

                using Workbook book = new Workbook();

                int i = 0;
                foreach (var chuck in lines.Chunk(1000000))
                {
                    Worksheet sheet = book.Worksheets[i];
                    sheet.Cells.ImportArray(chuck.ToArray(), 0, 0, true);

                    i++;
                }

                using var stream = new MemoryStream();
                book.Save(stream, SaveFormat.Xlsx);

                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("SystemError", ex.Message);
                return Page();
            }
        }

    }
}