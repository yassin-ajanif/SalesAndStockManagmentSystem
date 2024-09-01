using Avalonia.Media.Imaging;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace GetStartedApp.Helpers
{
    public static class ImageResizerHelper
    {
        public static async Task<Bitmap> ResizeImageAsync(string imagePath)
        {
            using (var fileStream = File.OpenRead(imagePath))
            {
                var bitmap = new Bitmap(fileStream);
                var ratio = (double)bitmap.PixelSize.Width / bitmap.PixelSize.Height;
                var newWidth = 400;
                var newHeight = (int)(newWidth / ratio);

                var resized = new RenderTargetBitmap(new PixelSize(newWidth, newHeight), new Vector(96, 96));

                using (var context = resized.CreateDrawingContext())
                {
                    context.DrawImage(bitmap, new Rect(0, 0, newWidth, newHeight));
                }

                return resized;
            }
        }


        public static long GetImageSizeInKb(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream);
                var sizeInBytes = memoryStream.Length;
                var sizeInKb = sizeInBytes / 1024; // Convert bytes to kilobytes
                return sizeInKb;
            }
        }

    }

}
