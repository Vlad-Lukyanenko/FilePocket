namespace FilePocket.Domain.Models
{
    public class VideoFrameModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] FrameBytes { get; set; } = [];
    }
}
