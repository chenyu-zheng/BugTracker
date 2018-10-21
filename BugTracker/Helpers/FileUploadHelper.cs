using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BugTracker.Helpers
{
    public class FileUploadHelper
    {
        public static string MD5String(HttpPostedFileBase file)
        {
            Stream stream = file.InputStream;
            stream.Position = 0;

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] buffer = md5.ComputeHash(stream);

            return BitConverter.ToString(buffer).Replace("-", string.Empty);
        }

        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
                return false;
            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;
            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                           ImageFormat.Png.Equals(img.RawFormat) ||
                           ImageFormat.Gif.Equals(img.RawFormat);
                }
            }
            catch
            {
                return false;
            }
        }

        public static void DeleteFile(string filePath)
        {
            try
            {
                System.IO.File.Delete(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}