﻿using Aspose.Cells.Drawing;
using Aspose.Cells;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using SampleApp.Models;
using SampleApp.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SampleApp.Services;

namespace SampleApp.Pages
{
    public class ValidateFileDataPhoneNumberModel : PageModel
    {
        private readonly IFileProvider _fileProvider;
        private readonly IValidateFilePhoneNumber _validateFilePhoneNumber;
        public ValidateFileDataPhoneNumberModel(IFileProvider fileProvider, IValidateFilePhoneNumber validateFilePhoneNumber)
        {
            _fileProvider = fileProvider;
            _validateFilePhoneNumber = validateFilePhoneNumber;
        }

        [BindProperty]
        public List<FileItem> SourceFiles { get; set; }

        [BindProperty]
        public bool RemoveDuplication { get; set; }

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
        }

        public IActionResult OnPostHandler(IEnumerable<FileItem> SourceFiles, bool RemoveDuplication)
        {
            SourceFiles = SourceFiles.Where(x => x.IsSelected).ToList();

            if (!SourceFiles.Any())
            {
                ModelState.AddModelError("Source file not selected", "Chưa chọn file");
            }

            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.SelectMany(x => x.Value.Errors).Select(e => e.ErrorMessage);
                return Page();
            }

            var content = _validateFilePhoneNumber.ValidateFile(SourceFiles.Select(x => x.FilePath));

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8");
        }
    }
}