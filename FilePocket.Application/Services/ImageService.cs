using FilePocket.Contracts.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
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
            using var image = Image.Load<Rgba32>(imageBytes);
            image.Mutate(x => x.Resize(width, height));

            image.ProcessPixelRows(accessor =>
            {
                var white = Color.White;

                for (int y = 0; y < accessor.Height; y++)
                {
                    var pixelRow = accessor.GetRowSpan(y);

                    for (int x = 0; x < pixelRow.Length; x++)
                    {
                        ref Rgba32 pixel = ref pixelRow[x];
                        if (pixel.A <= 25)
                        {
                            pixel = white;
                        }
                    }
                }
            });

            using var ms = new MemoryStream();
            var encoder = new JpegEncoder() { Quality = 90 };
            image.Save(ms, encoder);

            return ms.ToArray();
        }
    }
}
