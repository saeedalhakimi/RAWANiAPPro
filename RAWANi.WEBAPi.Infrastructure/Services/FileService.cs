using RAWANi.WEBAPi.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadsFolderPath;

        public FileService(string uploadsFolderPath)
        {
            _uploadsFolderPath = uploadsFolderPath;

            // Ensure the uploads folder exists
            if (!Directory.Exists(_uploadsFolderPath))
            {
                Directory.CreateDirectory(_uploadsFolderPath);
            }
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
        {
            var uniqueFileName = GenerateUniqueFileName(fileName);
            var filePath = Path.Combine(_uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

        public string GenerateUniqueFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            return uniqueName;
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_uploadsFolderPath, Path.GetFileName(filePath)); // Ensure correct path

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }

    }
}
