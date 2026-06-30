using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MyTCSCAN.Services
{
    public class ImageService : IImageService
    {
        private readonly IConfiguration _config;
        private readonly string _basePath;

        public ImageService(IConfiguration config)
        {
            _config = config;
            _basePath = _config["AppSettings:ImageBasePath"] ?? "";
        }

        public string GetImageFullPath(string storedPath)
        {
            if (string.IsNullOrEmpty(storedPath))
                return null;
n            if (Path.IsPathRooted(storedPath)) return storedPath;
n            return Path.Combine(_basePath, storedPath.TrimStart('\\', '/').Replace('/', Path.DirectorySeparatorChar));
        }

        public string GetBase64Image(string storedPath)
        {
            try
            {
                var full = GetImageFullPath(storedPath);
                if (string.IsNullOrEmpty(full) || !File.Exists(full)) return null;
                var bytes = File.ReadAllBytes(full);
                return Convert.ToBase64String(bytes);
            }
            catch
            {
                return null;
            }
        }
    }
}
