using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace GetStartedApp.Helpers
{
   
        public static class ImageHelper
        {
         
        public static Bitmap LoadFromResource(Uri resourceUri)
         {
         Bitmap bitmapReturned = null;
         
         try { bitmapReturned = new Bitmap(AssetLoader.Open(resourceUri)); }
         
         catch (Exception ex){ }
         
             return bitmapReturned;
         }

          
        public static async Task<Bitmap?> LoadFromWeb(Uri url)
            {
                using var httpClient = new HttpClient();
                try
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var data = await response.Content.ReadAsByteArrayAsync();
                    return new Bitmap(new MemoryStream(data));
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
                    return null;
                }
            }


        public static bool IsTheOriginalImageDifferentFromCurrent(Bitmap Image1, Bitmap Image2)
        {

            if (Image1 == null || Image2 == null) return false;

            return Image1.Size != Image2.Size;
        }


    }
    
}
