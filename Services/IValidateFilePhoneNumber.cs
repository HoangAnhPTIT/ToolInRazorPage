using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.IO;

namespace SampleApp.Services
{
    public interface IValidateFilePhoneNumber
    {
        public byte[] ValidateFile(ModelStateDictionary modelState, IEnumerable<string> filePaths);
    }
}