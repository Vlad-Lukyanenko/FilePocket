﻿

using SixLabors.ImageSharp;

namespace FilePocket.Contracts.Services
{
    public interface IImageService
    {
        Image GetImage(string path);
        byte[] ResizeImage(byte[] imageBytes, int width, int height);
    }
}