using Aspose.Cells;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SampleApp.Models;
using SampleApp.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SampleApp.Pages
{
    public class GetDistinctDataFromExcelFilesModel : PageModel
    {
        private readonly IFileProvider _fileProvider;
        private readonly ILogger<GetDistinctDataFromExcelFilesModel> _logger;

        public GetDistinctDataFromExcelFilesModel(IFileProvider fileProvider, ILogger<GetDistinctDataFromExcelFilesModel> logger)
        {
            _fileProvider = fileProvider;
            _logger = logger;
        }

        [BindProperty]
        public List<FileItem> SourceFiles { get; set; }

        [BindProperty]
        public List<FileItem> TargetFiles { get; set; }

        [BindProperty]
        public bool IsCompareProcessing { get; set; }

        public List<string> ErrorMessage { get; set; }

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

        public IActionResult OnPostCompare(IEnumerable<FileItem> SourceFiles, IEnumerable<FileItem> TargetFiles)
        {
            try
            {
                const string PHONENUMBER = "phoneNumber";
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

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

                List<string> validatedHeader = new();

                List<List<string>> headers = new();
                for (var f = 0; f < TargetFiles.Count(); f++)
                {
                    using var package = new ExcelPackage(new FileInfo(TargetFiles.ElementAt(f).FilePath));
                    var ws = package.Workbook.Worksheets[0];
                    var header = ExcelWorksheetExtension.GetHeaderColumns(ws);
                    if (header.Length > 0 && header.First() != PHONENUMBER)
                    {
                        ModelState.AddModelError("First cell must be phoneNumber", $"File thứ {f + 1}: Cột đầu tiên không khải {PHONENUMBER}");
                    }
                    headers.Add(header.ToList());
                }

                validatedHeader.AddRange(headers.First());

                if (TargetFiles.Count() > 1)
                {
                    for (var inx = 0; inx < headers.Count - 1; inx++)
                    {
                        if (!Enumerable.SequenceEqual(headers[inx], headers[inx + 1]))
                        {
                            ModelState.AddModelError("Target file not match headers", $"File thứ {inx + 1} không khớp header với file thứ {inx + 2} ở file so sánh");
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    ErrorMessage = ModelState.SelectMany(x => x.Value.Errors).Select(e => e.ErrorMessage).ToList();
                    IsCompareProcessing = false;
                    return Page();
                }

                HashSet<string> set = new HashSet<string>();
                List<dynamic> lines = new List<dynamic>();

                foreach (var file in SourceFiles)
                {
                    using var package = new ExcelPackage(new FileInfo(file.FilePath));
                    var ws = package.Workbook.Worksheets[0];

                    for (int rw = 1; rw <= ws.Dimension.End.Row; rw++)
                    {
                        var value = ws.Cells[rw, 1].Value;
                        if (value != null && !string.IsNullOrEmpty(value.ToString()) && Regex.Match(value.ToString(), Constant.RegexPhoneNumber).Success)
                        {
                            var phoneNumber = PhoneNumberUtility.StandardizedPhoneNumber(value);
                            if (phoneNumber.Length == 10 || phoneNumber.Length == 11)
                                set.Add(phoneNumber.ToString());
                        }
                    }
                }

                foreach (var file in TargetFiles)
                {
                    using var package = new ExcelPackage(new FileInfo(file.FilePath));
                    var ws = package.Workbook.Worksheets[0];
                    for (int rw = 2; rw <= ws.Dimension.End.Row; rw++)
                    {
                        dynamic obj = new ExpandoObject();
                        for (int h = 0; h < validatedHeader.Count; h++)
                        {
                            var value = ws.Cells[rw, h + 1].Value;
                            string valueHeader = validatedHeader[h];
                            ((IDictionary<String, Object>)obj)[valueHeader] = value;
                        }

                        var valuePhoneNumer = ((IDictionary<String, Object>)obj)[validatedHeader[0]];
                        if (valuePhoneNumer != null && !string.IsNullOrEmpty(valuePhoneNumer.ToString()) && Regex.Match(valuePhoneNumer.ToString(), Constant.RegexPhoneNumber).Success)
                        {
                            var phoneNumber = PhoneNumberUtility.StandardizedPhoneNumber(valuePhoneNumer);
                            if (phoneNumber.Length == 10 || phoneNumber.Length == 11)
                                if (set.Add(phoneNumber))
                                {
                                    ((IDictionary<String, Object>)obj)[validatedHeader[0]] = phoneNumber;
                                    lines.Add(obj);
                                }
                        }
                    }
                }

                using Workbook book = new Workbook();
                var chuckes = lines.Chunk(1000000);
                int i = 0;
                foreach (var chuck in chuckes)
                {
                    Worksheet sheet = book.Worksheets[i];
                    sheet.Cells.ImportCustomObjects(chuck.ToArray(), 0, 0, new ImportTableOptions
                    {
                    });

                    i++;
                    if (i <= chuckes.Count() - 1)
                    {
                        book.Worksheets.Add();
                    }
                }

                using var stream = new MemoryStream();
                book.Save(stream, SaveFormat.Xlsx);

                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8");
            }
            catch (Exception ex)
            {
                ErrorMessage.Add(ex.Message);
                return Page();
            }
        }
    }
}