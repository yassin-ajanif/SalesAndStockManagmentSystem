using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
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

    public static class ImageConverter
    {

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {

            try{ 
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream);
                return memoryStream.ToArray();
            }
            }

            catch (Exception e) { return null; }
            }

        public static Bitmap ByteArrayToBitmap(byte[] byteArray)
        {
            try { 
            using (var memoryStream = new MemoryStream(byteArray))
            {
                return new Bitmap(memoryStream);
            }
            }

            catch (Exception e) { return null; }
        }
    }

}
