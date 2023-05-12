using System.Collections.Generic;
using System.IO;

namespace SampleApp.Services
{
    public interface IValidateFilePhoneNumber
    {
        public byte[] ValidateFile(IEnumerable<string> filePaths);
    }
}