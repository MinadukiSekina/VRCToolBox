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
                Uri uri = new Uri(path, UriKind.Relative);
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
    }
}