using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Music.WPF.Converters
{
    public sealed class FilePathToImageConverter : IValueConverter
    {
        private const string DEFAULT_ALBUM_ICON = "pack://application:,,,/Music.WPF;component/Icons/default_album_icon.png";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage bitmapImage;

            if (value is string filePath && !string.IsNullOrEmpty(filePath))
            {
                if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg"))
                {
                    bitmapImage = new BitmapImage(new Uri(filePath));
                }
                else if (filePath.EndsWith(".mp3"))
                {
                    using var file = TagLib.File.Create(filePath);

                    if (file?.Tag.Pictures.Length > 0)
                    {
                        var picture = file.Tag.Pictures[0];
                        using var memoryStream = new MemoryStream(picture.Data.Data);
                        bitmapImage = new Bitmap(memoryStream).ToBitmapImage();
                    }
                    else
                    {
                        bitmapImage = new BitmapImage(new Uri(DEFAULT_ALBUM_ICON));
                    }
                }
                else
                {
                    bitmapImage = new BitmapImage(new Uri(DEFAULT_ALBUM_ICON));
                }
            }
            else
            {
                bitmapImage = new BitmapImage(new Uri(DEFAULT_ALBUM_ICON));
            }

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal static class BitmapExtension
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using var memory = new MemoryStream();

            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;

        }
    }
}
