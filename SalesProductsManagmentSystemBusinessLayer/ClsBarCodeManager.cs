using System;
using System.IO;
using System.Threading;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.SkiaSharp.Rendering;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using Svg.Skia;
using System.Text.RegularExpressions;
using System.Diagnostics;






namespace SalesProductsManagmentSystemBusinessLayer
{
    public static class ClsBarCodeManager
    {
       


        public static bool IsStringAPositiveIntegerNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            // Try to parse the string as a double
            long result;
            return long.TryParse(input, out result) && result>0;
        }

        public static string GenerateBarcodeSvg(string content)
        {

            // Generate barcode image using ZXing and SkiaSharp
            var barcodeWriter = new BarcodeWriterSvg
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 1,
                    Height = 30,
                    Margin = 5,
                },


                Renderer = new ZXing.Rendering.SvgRenderer()
                {
                    FontSize = 10,
                }


            };



            // Write the barcode to an SVG string
            var barcodeSvg = barcodeWriter.Write(content);


            return barcodeSvg.Content;
        }
   

        // this function is test but its prive for this reason you don't see green dot above it
        public static SKImage AddBorderToImage(SKImage image, int borderWidth, SKColor borderColor)
        {
            var maximalBorderWidth = int.MaxValue / 1000;

            if (image == null || borderWidth < 0 || borderWidth >= int.MaxValue || borderColor == null || borderWidth >= maximalBorderWidth) return null;
           
                try
            {
                // Create a new bitmap with space for the border
                var bitmap = new SKBitmap(image.Width + 2 * borderWidth, image.Height + 2 * borderWidth);

                using (var canvas = new SKCanvas(bitmap))
                {
                    // Draw the border
                    var paint = new SKPaint { Color = borderColor, IsStroke = false };
                    canvas.DrawRect(0, 0, bitmap.Width, bitmap.Height, paint);

                    // Draw the original image onto the bitmap, offset by the border width
                    canvas.DrawImage(image, borderWidth, borderWidth);
                }

                // Convert the SKBitmap to an SKImage and return it
                return SKImage.FromBitmap(bitmap);
            }
            catch (Exception ex)
            {
                // Log the exception or handle as needed
                Console.WriteLine($"Exception: {ex.Message}");
                return null; // Return null in case of an exception
            }
        }



        public static string AddBorderToSvg(string svgContent, float strokeWidth, string strokeColor)
        {
            // Define the border rect SVG element
            string borderRect = $@"
        <rect x=""0"" y=""0"" width=""100%"" height=""110%"" 
              style=""fill:none;stroke:{strokeColor};stroke-width:{strokeWidth};""/>";

            // Insert the border rect before the closing </svg> tag
            string updatedSvg = Regex.Replace(svgContent, @"</svg>", borderRect + "\n</svg>", RegexOptions.IgnoreCase);

            return updatedSvg;
        }

        public static SKBitmap AddBarcodeToPreview(
         string content,
         int numRows = 16,
         int numColumns = 7,
         float spacingMillimeters = 0f, // Spacing in millimeters
         int startRow = 1,
         int startColumn = 3,
         int numBarcodes = 3,
        float offsetXMillimeters = 10f,  // Horizontal offset in millimeters
        float offsetYMillimeters = 10f   // Vertical offset in millimeters
  )
        {
            if (!IsStringAPositiveIntegerNumber(content) || content.Length > 18) return null;
            if (numRows < 0 || numColumns < 0 || spacingMillimeters < 0 || startRow < 0 || startColumn < 0 || offsetXMillimeters < 0 || offsetYMillimeters < 0) return null;

            try
            {
                // Convert millimeter values to points
                float spacing = spacingMillimeters * (float)(96 / 25.4); // 1 inch = 25.4 millimeters, assuming 96 DPI
                float offsetX = offsetXMillimeters * (float)(96 / 25.4);
                float offsetY = offsetYMillimeters * (float)(96 / 25.4);

                // Generate the barcode SVG content
                string orignalBarcodSvg = GenerateBarcodeSvg(content);
               // string barcodeSvgWidthBorder = AddBorderToSvg(orignalBarcodSvg, 1f, "white");
                string barcodeSvgWidthBorder = orignalBarcodSvg;

                if (string.IsNullOrEmpty(barcodeSvgWidthBorder)) return null;

                // Load the SVG content into an SKSvg object
                var svg = new SKSvg();
                
                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(barcodeSvgWidthBorder)))
                {
                    svg.Load(stream);
                }
                var svgPicture = svg.Picture;
                if (svgPicture == null) return null;

                // Set the size for an A4 sheet (in points)
                float pageWidth = 595f;
                float pageHeight = 842f;

                // Calculate the size for the grid cells
                float cellWidth = (pageWidth - (numColumns + 1) * spacing) / numColumns;
                float cellHeight = (pageHeight - (numRows + 1) * spacing) / numRows;



                // Create a new image-backed canvas
                SKBitmap image = new SKBitmap((int)pageWidth, (int)pageHeight);
                using (SKCanvas canvas = new SKCanvas(image))
                {
                    // Draw the white background
                    canvas.Clear(SKColors.White);

                    // Draw the grid and place the barcodes
                    int barcodeCount = 0;
                    for (int row = 0; row < numRows; row++)
                    {
                        for (int col = 0; col < numColumns; col++)
                        {
                            float x = spacing + col * (cellWidth + spacing);
                            float y = spacing + row * (cellHeight + spacing);
                            var rect = new SKRect(x, y, x + cellWidth, y + cellHeight);
                            canvas.DrawRect(rect, new SKPaint { Color = SKColors.Red, IsStroke = true });

                            // If we're at the starting position and there are still barcodes to place, draw a barcode
                            if (row >= startRow && col >= startColumn && barcodeCount < numBarcodes)
                            {
                                // Adjust the position by the offset
                                //  var matrix = SKMatrix.CreateTranslation(x + offsetX, y + offsetY);
                                // canvas.DrawPicture(svgPicture, ref matrix);
                                float ScalingFactorOfBarCodeSvg_BasedOnItOriginalWidth = CalculateTheScalingFactorOfBarCodeSvg(svg);
                                var scaleMatrix = SKMatrix.CreateScale(ScalingFactorOfBarCodeSvg_BasedOnItOriginalWidth, 1);
                              
                                var combinedMatrix = scaleMatrix.PostConcat(SKMatrix.CreateTranslation(x + offsetX, y + offsetY));
                                canvas.DrawPicture(svgPicture, ref combinedMatrix);

                                barcodeCount++;

                                // Move to the next column or row
                                startColumn++;
                                if (startColumn >= numColumns)
                                {
                                    startColumn = 0;
                                    startRow++;
                                }
                            }
                        }
                    }

                    // Determine the path of the project folder
                    string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    // Construct the full path to the image file
                    string imageName = "A4GridPreview.svg";
                    string outputFilePath = Path.Combine(executablePath, imageName);

                    // Save the SVG content to a file
                    File.WriteAllText(outputFilePath, barcodeSvgWidthBorder);
                }

                // Return the generated SKBitmap
                return image;
            }
            catch (Exception ex)
            {
                // Handle exception
                return null;
            }
        }

       public static float CalculateTheScalingFactorOfBarCodeSvg(SKSvg svg)
        {
            // Get the SVG's dimensions
            var svgWidth = svg.Picture.CullRect.Width;
            
            float ScalingFactor = 1;
            if (svgWidth > 80 && svgWidth < 131) ScalingFactor = 0.6f;
            // when the svgwidth is higher than that is got multiplied by 2 , this is understood by a lot of tests
            else if (svgWidth > 131) ScalingFactor = 0.5f;
            // Calculate scale to fit SVG within PDF page
            float scaleX = ScalingFactor;

            return scaleX;
        }

        public static void GenerateBarCodePdf(
     string content,
     int numRows = 17,
     int numColumns = 6,
     float spacingMillimeters = 2f, // Spacing in millimeters
     int startRow = 1,
     int startColumn = 3,
     int numBarcodes = 3,
     float offsetXMillimeters = 10f,  // Horizontal offset in millimeters
     float offsetYMillimeters = 10f   // Vertical offset in millimeters
 )
        {
            if (!IsStringAPositiveIntegerNumber(content) || content.Length > 18) return;
            if (numRows < 0 || numColumns < 0 || spacingMillimeters < 0 || startRow < 0 || startColumn < 0 || offsetXMillimeters < 0 || offsetYMillimeters < 0) return;

            try
            {
                // Convert millimeter values to points
                float spacing = spacingMillimeters * (float)(96 / 25.4); // 1 inch = 25.4 millimeters, assuming 96 DPI
                float offsetX = offsetXMillimeters * (float)(96 / 25.4);
                float offsetY = offsetYMillimeters * (float)(96 / 25.4);

                // Generate the barcode SVG content
                string originalBarcodeSvg = GenerateBarcodeSvg(content);
                string barcodeSvgWidthBorder = originalBarcodeSvg; // Use scaled SVG

                if (string.IsNullOrEmpty(barcodeSvgWidthBorder)) return;

                // Load the SVG content into an SKSvg object
                var svg = new SKSvg();
                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(barcodeSvgWidthBorder)))
                {
                    svg.Load(stream);
                }
                var svgPicture = svg.Picture;
                if (svgPicture == null) return;

                // Set the size for an A4 sheet (in points)
                float pageWidth = 595f;
                float pageHeight = 842f;

                // Calculate the size for the grid cells
                float cellWidth = (pageWidth - (numColumns + 1) * spacing) / numColumns;
                float cellHeight = (pageHeight - (numRows + 1) * spacing) / numRows;

                // Create a new PDF document
                using (var stream = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"BarcodeGrid_{content}.pdf"), FileMode.Create))
                {
                    using (var document = SKDocument.CreatePdf(stream))
                    {
                        using (var canvas = document.BeginPage(pageWidth, pageHeight))
                        {
                            //var canvas = page.Canvas;

                            // Draw the white background
                            canvas.Clear(SKColors.White);

                            // Draw the grid and place the barcodes
                            int barcodeCount = 0;
                            for (int row = 0; row < numRows; row++)
                            {
                                for (int col = 0; col < numColumns; col++)
                                {
                                    float x = spacing + col * (cellWidth + spacing);
                                    float y = spacing + row * (cellHeight + spacing);
                                    var rect = new SKRect(x, y, x + cellWidth, y + cellHeight);
                                    canvas.DrawRect(rect, new SKPaint { Color = SKColors.LightGray, IsStroke = true });

                                    // If we're at the starting position and there are still barcodes to place, draw a barcode
                                    if (row >= startRow && col >= startColumn && barcodeCount < numBarcodes)
                                    {

                                        float ScalingFactorOfBarCodeSvg_BasedOnItOriginalWidth = CalculateTheScalingFactorOfBarCodeSvg(svg);
                                        var scaleMatrix = SKMatrix.CreateScale(ScalingFactorOfBarCodeSvg_BasedOnItOriginalWidth, 1);
                                        var combinedMatrix = scaleMatrix.PostConcat(SKMatrix.CreateTranslation(x + offsetX, y + offsetY));
                                        canvas.DrawPicture(svgPicture, ref combinedMatrix);

                                        barcodeCount++;

                                        // Move to the next column or row
                                        startColumn++;
                                        if (startColumn >= numColumns)
                                        {
                                            startColumn = 0;
                                            startRow++;
                                        }
                                    }
                                }
                            }
                        }

                        // Complete the PDF document
                        document.Close();

                        OpenPdfFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"BarcodeGrid_{content}.pdf"));
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void OpenPdfFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                };
                Process process = new Process
                {
                    StartInfo = processStartInfo
                };
                process.Start();
            }
            else
            {
                Console.WriteLine("The file does not exist.");
            }
        }


    }
}


