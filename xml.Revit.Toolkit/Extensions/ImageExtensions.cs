/* 作    者: xml
 ** 创建时间: 2024/2/16 20:26:06
 **
 ** Copyright 2024 by zedmoster
 ** Permission to use, copy, modify, and distribute this software in
 ** object code form for any purpose and without fee is hereby granted,
 ** provided that the above copyright notice appears in all copies and
 ** that both that copyright notice and the limited warranty and
 ** restricted rights notice below appear in all supporting
 ** documentation.
 */

using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace xml.Revit.Toolkit.Extensions;

/// <summary>
/// ImageExtensions
/// </summary>
public static class ImageExtensions
{
    /// <summary>
    /// 获取图片
    /// </summary>
    /// <param name="byteArray"></param>
    /// <returns></returns>
    public static BitmapImage GetImageBitmapSource(this byte[] byteArray)
    {
        using Stream stream = new MemoryStream(byteArray);
        BitmapImage image = new();
        stream.Position = 0;
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();
        return image;
    }

    /// <summary>
    /// Image 转换 BitmapSource
    /// </summary>
    /// <param name="image"> 资源图片</param>
    /// <returns>BitmapSource</returns>
    public static BitmapSource GetImageBitmapSource(this Image image)
    {
        if (image == null)
            return null;

        using MemoryStream stream = new MemoryStream();
        BitmapImage bitmapImage = new BitmapImage();
        image.Save(stream, ImageFormat.Png);
        stream.Position = 0;
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.UriSource = null;
        bitmapImage.StreamSource = stream;
        bitmapImage.EndInit();
        return bitmapImage;
    }

    /// <param name="source"></param>
    extension(BitmapSource source)
    {
        /// <summary>
        /// 32x32 转换为 16x16
        /// </summary>
        /// <returns></returns>
        public BitmapSource ResizeImage()
        {
            if (source == null)
            {
                return null;
            }

            var resizedImage = new TransformedBitmap(
                source,
                new ScaleTransform(16 / (double)source.PixelWidth, 16 / (double)source.PixelHeight)
            );

            return resizedImage;
        }

        /// <summary>
        /// BitmapSource to base64
        /// </summary>
        /// <returns></returns>
        public string ToBase64String()
        {
            var encoder = new PngBitmapEncoder();
            var frame = BitmapFrame.Create(source);
            encoder.Frames.Add(frame);
            using var stream = new MemoryStream();
            encoder.Save(stream);
            return Convert.ToBase64String(stream.ToArray());
        }
    }

    /// <summary>
    /// BitmapSource to base64
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static BitmapSource FromBase64String(this string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        var stream = new MemoryStream(bytes);
        return BitmapFrame.Create(stream);
    }
}
