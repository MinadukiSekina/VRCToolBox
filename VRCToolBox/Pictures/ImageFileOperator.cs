using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using SkiaSharp;

namespace VRCToolBox.Pictures
{

    /// <summary>
    /// Format of picture. base : SkiaSharp.
    /// </summary>
    public enum PictureFormat
    {
        /// <summary>
        /// The BMP format.
        /// </summary>
        [Description("BMP")]
        Bmp,

        /// <summary>
        /// The JPEG format.
        /// </summary>
        [Description("JPEG")]
        Jpeg,

        /// <summary>
        /// The PNG format.
        /// </summary>
        [Description("PNG")]
        Png,

        /// <summary>
        /// The WEBP (Lossy) format.
        /// </summary>
        [Description("WEBP(劣化あり)")]
        WebpLossy,

        /// <summary>
        /// The WEBP (Lossless) format.
        /// </summary>
        [Description("WEBP(劣化なし)")]
        WebpLossless,
    }

    internal class ImageFileOperator
    {
        internal static string GetBase64Image(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    using (FileStream fs = File.OpenRead(path))
                    {
                        byte[] bytes = new byte[fs.Length];
                        _ = fs.Read(bytes);
                        fs.Close();
                        return Convert.ToBase64String(bytes);
                    }
                }
                else
                {
                    Uri uri = new Uri(path, UriKind.Relative);
                    StreamResourceInfo resourceInfo = Application.GetResourceStream(uri);
                    try
                    {
                        byte[] bytes = new byte[resourceInfo.Stream.Length];
                        _ = resourceInfo.Stream.Read(bytes);
                        return Convert.ToBase64String(bytes);
                    }
                    catch (Exception ex)
                    {
                        // TODO Do something.
                    }
                    finally
                    {
                        resourceInfo.Stream.Close();
                        resourceInfo.Stream.Dispose();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        internal static BitmapImage GetDecodedImage(string path, int decodePixelWidth = 132, bool needDecode = true)
        {
            BitmapImage bitmapImage = new BitmapImage();
            
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    bitmapImage.Freeze();
                    return bitmapImage;
                }

                if (File.Exists(path))
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = fs;
                        if (needDecode) bitmapImage.DecodePixelWidth = decodePixelWidth;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                        fs.Close();
                    }
                }
                else
                {
                    var app = App.Current as App;
                    if (app is null)
                    {
                        bitmapImage.Freeze();
                        return bitmapImage;
                    }
                    return Directory.Exists(path) ? app.FolderImage : app.ErrorImage;
                }
                bitmapImage.Freeze();
                return bitmapImage;
            }
            catch (Exception ex)
            {
                var app = App.Current as App;
                if (app is null)
                {
                    bitmapImage.Freeze();
                    return bitmapImage;
                }
                return app.ErrorImage;
            }
        }
        internal static BitmapImage GetDecodedImageFromAppResource(string uri, int decodePixelWidth = 132)
        {
            BitmapImage bitmapImage = new BitmapImage();
            StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri(uri, UriKind.Relative));
            try
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = resourceInfo.Stream;
                bitmapImage.DecodePixelWidth = decodePixelWidth;
                bitmapImage.EndInit();
            }
            catch (Exception ex)
            {
                // TODO Do something.
            }
            finally
            {
                resourceInfo.Stream.Close();
                resourceInfo.Stream.Dispose();
            }
            bitmapImage.Freeze();
            return bitmapImage;
        }
        internal static byte[]? GetFitSizeImageBytes(string imagePath, long maximumLength)
        {
            try
            {
                // Check existance and open stream.
                if (!File.Exists(imagePath)) return null;
                using FileStream fileStream = File.OpenRead(imagePath);

                // When under the limit.
                if(fileStream.Length <= maximumLength)
                {
                    byte[] bytes = new byte[fileStream.Length];
                    _ = fileStream.Read(bytes, 0, bytes.Length);
                    fileStream.Close();
                    return bytes;
                }

                // When over the limit.
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption  = BitmapCacheOption.OnLoad;
                image.StreamSource = fileStream;
                image.EndInit();
                image.Freeze();

                bool widthIsLarge = image.DecodePixelWidth >= image.DecodePixelHeight;
                int decodeSize = widthIsLarge ? image.PixelWidth / 2 : image.PixelHeight / 2;
                BitmapEncoder encoder;
                switch (Path.GetExtension(imagePath))
                {
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case ".jpeg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ".jpg":
                        encoder= new JpegBitmapEncoder();
                        break;
                    default:
                        encoder = new BmpBitmapEncoder();
                        break;
                }
                // Reload image.
                bool sizeIsOver = true;
                while (sizeIsOver)
                {
                    image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption |= BitmapCacheOption.OnLoad;
                    image.StreamSource = fileStream;
                    if (widthIsLarge)
                    {
                        image.DecodePixelWidth = decodeSize;
                    }
                    else
                    {
                        image.DecodePixelHeight = decodeSize;
                    }
                    image.EndInit();
                    image.Freeze();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    using var ms = new MemoryStream();
                    encoder.Save(ms);
                    byte[] data = ms.ToArray();
                    sizeIsOver = data.Length > maximumLength;
                    if (!sizeIsOver) 
                    { 
                        fileStream.Close();
                        return data; 
                    }
                    decodeSize <<= 2;
                }
                fileStream.Close();
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal static void SaveRotatedPhoto(string path, float rotation)
        {
            if (!File.Exists(path)) return;
            var original = new BitmapImage();
            using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                original.BeginInit();
                original.CacheOption = BitmapCacheOption.OnLoad;
                original.StreamSource = fs;
                original.EndInit();
                original.Freeze();

                var transformedBitmap = new TransformedBitmap();
                transformedBitmap.BeginInit();
                transformedBitmap.Source = original;
                transformedBitmap.Transform = new RotateTransform(rotation);
                transformedBitmap.EndInit();
                transformedBitmap.Freeze();

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(transformedBitmap));
                encoder.Save(fs);
            }
        }

        // Convert image to webp.
        internal static async Task ConvertToWebpAsync(string destDir, string filePath, int quality = 100)
        {
            if (!File.Exists(filePath)) return;

            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

            var destPath = Path.Combine(destDir, $"{Guid.NewGuid()}.webp");

            if (File.Exists(destPath)) return;

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var baseImage = SKBitmap.Decode(fs);

            var convertedData = baseImage.Encode(SKEncodedImageFormat.Webp, quality);
            await File.WriteAllBytesAsync(destPath, convertedData.ToArray());

        }

        internal static SKImage GetSKImage(string filePath)
        {
            if(!File.Exists(filePath)) throw new FileNotFoundException();
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var baseImage = SKBitmap.Decode(fs);
            return SKImage.FromBitmap(baseImage);
        }

        internal static SKBitmap GetSKBitmap(string filePath)
        {
            if(!File.Exists(filePath)) throw new FileNotFoundException();
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return SKBitmap.Decode(fs);
        }

        internal static SKData GetSKData(string filePath)
        {
            if(!File.Exists(filePath)) throw new FileNotFoundException();
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return SKData.Create(fs);
        }

        internal static SKPixmap GetSKPixmap(string filePath)
        {
            var bitmap = GetSKBitmap(filePath);
            return bitmap.PeekPixels();
        }
        internal static SKBitmap GetConvertedImage(Interface.IImageConvertTarget target)
        {
            
            // リサイズ処理
            using var resizedBitmap = Resize(target.RawData.Value, target.ResizeOptions);

            switch (target.ConvertFormat.Value)
            {
                case PictureFormat.Jpeg:
                    return SKBitmap.Decode(ConvertToJPEG(resizedBitmap, target.JpegEncoderOptions));

                case PictureFormat.Png:
                    return SKBitmap.Decode(ConvertToPNG(resizedBitmap, target.PngEncoderOptions));

                case PictureFormat.WebpLossy:
                    return  SKBitmap.Decode(ConvertToWEBP(resizedBitmap, target.WebpLossyEncoderOptions));

                case PictureFormat.WebpLossless:
                    return  SKBitmap.Decode(ConvertToWEBP(resizedBitmap, target.WebpLosslessEncoderOptions));

                default:
                    throw new NotSupportedException("選択された変換後の形式への変換は実装されていません。");
            }

        }
        internal static SKBitmap GetConvertedImage(Interface.IImageConvertTargetWithReactiveImage target)
        {
            // リサイズ処理
            using var resizedBitmap = Resize(target.RawData.Value, target.ResizeOptions);

            switch (target.ConvertFormat.Value)
            {
                case PictureFormat.Jpeg:
                    return SKBitmap.Decode(ConvertToJPEG(resizedBitmap, target.JpegEncoderOptions));

                case PictureFormat.Png:
                    return SKBitmap.Decode(ConvertToPNG(resizedBitmap, target.PngEncoderOptions));

                case PictureFormat.WebpLossy:
                    return SKBitmap.Decode(ConvertToWEBP(resizedBitmap, target.WebpLossyEncoderOptions));

                case PictureFormat.WebpLossless:
                    return SKBitmap.Decode(ConvertToWEBP(resizedBitmap, target.WebpLosslessEncoderOptions));

                default:
                    throw new NotSupportedException("選択された変換後の形式への変換は実装されていません。");
            }

        }

        internal static SKData GetConvertedData(Interface.IImageConvertTarget target)
        {
            // リサイズ処理
            using var resizedBitmap = Resize(target.RawData.Value, target.ResizeOptions);

            switch (target.ConvertFormat.Value)
            {
                case PictureFormat.Jpeg:
                    return ConvertToJPEG(resizedBitmap, target.JpegEncoderOptions);

                case PictureFormat.Png:
                    return ConvertToPNG(resizedBitmap, target.PngEncoderOptions);

                case PictureFormat.WebpLossy:
                    return ConvertToWEBP(resizedBitmap, target.WebpLossyEncoderOptions);

                case PictureFormat.WebpLossless:
                    return ConvertToWEBP(resizedBitmap, target.WebpLosslessEncoderOptions);

                default:
                    throw new NotSupportedException("選択された変換後の形式への変換は実装されていません。");
            }
        }

        private static SKBitmap Resize(SKData baseData, Interface.IResizeOptions options)
        {
            var bitmap = SKBitmap.Decode(baseData);
            if (options.ScaleOfResize.Value == 1f) return bitmap;

            var newWidth  = (int)(bitmap.Width * options.ScaleOfResize.Value);
            var newHeight =(int)(bitmap.Height * options.ScaleOfResize.Value);
            var info      = bitmap.Info.WithSize(newWidth, newHeight);
            return bitmap.Resize(info, (SKFilterQuality)options.ResizeMode.Value);
        }
        private static SKData ConvertToJPEG(SKBitmap bitmap, Interface.IJpegEncoderOptions options)
        {
            var option = new SKJpegEncoderOptions(options.Quality.Value, (SKJpegEncoderDownsample)options.DownSample.Value, (SKJpegEncoderAlphaOption)options.AlphaOption.Value);
            using var pixmap = bitmap.PeekPixels();
            return pixmap.Encode(option);
        }
        private static SKData ConvertToPNG(SKBitmap bitmap, Interface.IPngEncoderOptions options)
        {
            var option = new SKPngEncoderOptions((SKPngEncoderFilterFlags)options.PngFilter.Value, options.ZLibLevel.Value);
            using var pixmap = bitmap.PeekPixels();
            return pixmap.Encode(option);
        }
        private static SKData ConvertToWEBP(SKBitmap bitmap, Interface.IWebpEncoderOptions options)
        {
            var sw = new System.Diagnostics.Stopwatch();
            var twt = new System.Diagnostics.TextWriterTraceListener($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\WEBP_Encode.txt");
            System.Diagnostics.Trace.Listeners.Add(twt);
            System.Diagnostics.Trace.WriteLine("WEBP Start!");
            sw.Start();
            var option = new SKWebpEncoderOptions((SKWebpEncoderCompression)options.WebpCompression.Value, options.Quality.Value);
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"{sw.ElapsedMilliseconds}");
            System.Diagnostics.Trace.WriteLine("Peek Pixels");
            sw.Start();
            using var pixmap = bitmap.PeekPixels();
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"{sw.ElapsedMilliseconds}");
            System.Diagnostics.Trace.WriteLine("Encode Start!");
            sw.Start();
            var data = pixmap.Encode(option);
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"{sw.ElapsedMilliseconds}");
            System.Diagnostics.Trace.WriteLine("Encode End!");
            System.Diagnostics.Trace.Flush();
            return data;
        }
    }
}
