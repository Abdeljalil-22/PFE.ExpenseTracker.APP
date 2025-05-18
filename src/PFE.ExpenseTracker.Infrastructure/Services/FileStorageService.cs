using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _uploadDirectory;

        public FileStorageService(IConfiguration configuration)
        {
            _uploadDirectory = configuration["FileStorage:UploadDirectory"] ?? "uploads";
            Directory.CreateDirectory(_uploadDirectory);
        }

        public async Task<Attachment> SaveFileAsync(IFormFile file, Guid expenseId)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var relativePath = Path.Combine("expenses", expenseId.ToString(), fileName);
            var fullPath = Path.Combine(_uploadDirectory, relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new Attachment
            {
                FileName = file.FileName,
                FilePath = relativePath,
                ContentType = file.ContentType,
                FileSize = file.Length,
                ExpenseId = expenseId
            };
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_uploadDirectory, filePath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found", filePath);

            return await File.ReadAllBytesAsync(fullPath);
        }

        public Task DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_uploadDirectory, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }
    }
}
