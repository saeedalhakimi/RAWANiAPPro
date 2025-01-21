using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName);
        string GenerateUniqueFileName(string fileName);
    }
}
