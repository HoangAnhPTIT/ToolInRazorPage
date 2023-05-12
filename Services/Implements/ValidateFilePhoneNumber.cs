using Aspose.Cells;
using OfficeOpenXml;
using SampleApp.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SampleApp.Services.Implements
{
    public class ValidateFilePhoneNumber : IValidateFilePhoneNumber
    {
        public byte[] ValidateFile(IEnumerable<string> filePaths)
        {
            HashSet<string> set = new();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            foreach (var filePath in filePaths)
            {
                using var package = new ExcelPackage(new FileInfo(filePath));
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

            using Workbook book = new();
            var chucks = set.ToList().Chunk(1000000);
            int i = 0;
            foreach (var chuck in chucks)
            {
                Worksheet sheet = book.Worksheets[i];
                sheet.Cells.ImportArray(chuck.ToArray(), 0, 0, true);

                i++;
                if(i <= chucks.Count() - 1)
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