namespace MyTCSCAN.Services
{
    public interface IImageService
    {
        string GetBase64Image(string relativeOrAbsolutePath);
        string GetImageFullPath(string storedPath);
    }
}
