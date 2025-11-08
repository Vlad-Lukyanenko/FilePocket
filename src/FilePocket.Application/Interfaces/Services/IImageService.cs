using FilePocket.Domain.Models;
using SixLabors.ImageSharp;

namespace FilePocket.Application.Interfaces.Services
{
    public interface IImageService
    {
        Image GetImage(string path);
        public Image GetImage(byte[] imageBytes);
        byte[] ResizeImage(byte[] imageBytes, int width, int height);
        VideoFrameModel ExtractFirstFrame(string path);
    }
}
