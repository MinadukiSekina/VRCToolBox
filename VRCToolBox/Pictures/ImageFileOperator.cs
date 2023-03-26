﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using System.Windows.Media.Imaging;

namespace VRCToolBox.Pictures
{
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
        internal static BitmapImage GetDecodedImage(string path, int decodePixelWidth = 132)
        {
            BitmapImage bitmapImage = new BitmapImage();

            if (string.IsNullOrWhiteSpace(path)) return bitmapImage;

            if (File.Exists(path))
            {
                using (FileStream fileStream = File.OpenRead(path))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = fileStream;
                    bitmapImage.DecodePixelWidth = decodePixelWidth;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    fileStream.Close();
                }
            }
            else
            {
                Uri uri = new Uri(Directory.Exists(path) ? Settings.ProgramConst.FolderImage : Settings.ProgramConst.LoadErrorImage, UriKind.Relative);
                StreamResourceInfo resourceInfo = Application.GetResourceStream(uri);
                try
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = resourceInfo.Stream;
                    bitmapImage.DecodePixelWidth = decodePixelWidth;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
                catch(Exception ex)
                {
                    // TODO Do something.
                }
                finally
                {
                    resourceInfo.Stream.Close();
                    resourceInfo.Stream.Dispose();
                }
            }
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
    }
}
