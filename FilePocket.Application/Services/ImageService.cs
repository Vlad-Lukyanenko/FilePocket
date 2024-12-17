using FilePocket.Contracts.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using FilePocket.Domain.Models;
using OpenCvSharp;


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

        public VideoFrameModel ExtractFirstFrame(string path)
        {
            using var capture = new VideoCapture(path);
            if (!capture.IsOpened())
            {
                throw new Exception("Failed to open video file.");
            }

            using var frame = new Mat();

            capture.Read(frame);

            if (!frame.Empty())
            {
                return new VideoFrameModel
                {
                    Width = frame.Width,
                    Height = frame.Height,
                    FrameBytes = frame.ToBytes()
                };
            }

            return new VideoFrameModel();
        }
    }


}
