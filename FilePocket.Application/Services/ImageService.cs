using FilePocket.Contracts.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;


namespace FilePocket.Application.Services
{
    public class ImageService : IImageService
    {
        public Image GetImage(string path)
        {
            return Image.Load(path);
        }

        public byte[] ResizeImage(byte[] imageBytes, int width, int height)
        {
            using var image = Image.Load(imageBytes);
            image.Mutate(x => x.Resize(width, height));

            using var ms = new MemoryStream();
            image.Save(ms, new JpegEncoder());
            return ms.ToArray();
        }
    }
}
