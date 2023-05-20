using Aspose.Cells;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OfficeOpenXml;
using SampleApp.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SampleApp.Services.Implements
{
    public class ValidateFilePhoneNumber : IValidateFilePhoneNumber
    {
        public byte[] ValidateFile(ModelStateDictionary modelState, IEnumerable<string> filePaths)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            const string PHONENUMBER = "phoneNumber";

            List<string> validatedHeader = new();

            List<List<string>> headers = new();
            for (var f = 0; f < filePaths.Count(); f++)
            {
                using var package = new ExcelPackage(new FileInfo(filePaths.ElementAt(f)));
                var ws = package.Workbook.Worksheets[0];
                var header = ExcelWorksheetExtension.GetHeaderColumns(ws);
                if (header.Length > 0 && header.First() != PHONENUMBER)
                {
                    modelState.AddModelError("First cell must be phoneNumber", $"File thứ {f + 1}: Cột đầu tiên không khải {PHONENUMBER}");
                }
                headers.Add(header.ToList());
            }

            validatedHeader.AddRange(headers.First());

            HashSet<string> set = new();
            Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

            foreach (var filePath in filePaths)
            {
                using var package = new ExcelPackage(new FileInfo(filePath));
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
                        {
                            ((IDictionary<String, Object>)obj)[validatedHeader[0]] = phoneNumber;
                            data.TryAdd(phoneNumber, obj);
                        }
                    }
                }
            }

            using Workbook book = new();
            var chucks = data.Values.ToList().Chunk(1000000);
            int i = 0;
            foreach (var chuck in chucks)
            {
                Worksheet sheet = book.Worksheets[i];
                sheet.Cells.ImportCustomObjects(chuck.ToArray(), 0, 0, new ImportTableOptions
                {
                });
                i++;
                if (i <= chucks.Count() - 1)
                {
                    book.Worksheets.Add();
                }
            }

            using var stream = new MemoryStream();
            book.Save(stream, SaveFormat.Xlsx);

            return stream.ToArray();
        }
    }
}