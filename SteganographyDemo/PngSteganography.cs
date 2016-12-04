using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steganography;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
namespace SteganographyDemo
{
    class PngSteganography : ISteganography<BitmapImage, BitmapSource>
    {

        ISteganography<byte[,], bool[,]> RawImageSteganography = new ImageSteganography();

        public BitmapSource Find(BitmapImage ModifiedMsg)
        {
            var modifiedBitmapSource = ModifiedMsg as BitmapSource;

            var height = modifiedBitmapSource.PixelHeight;
            var width = modifiedBitmapSource.PixelWidth;
            var bytesPerPixel = 3;
            var stride = width * bytesPerPixel;

            var pixelData = new byte[ stride * height ];
            if (modifiedBitmapSource.Format != PixelFormats.Rgb24)
                modifiedBitmapSource = new FormatConvertedBitmap(modifiedBitmapSource, PixelFormats.Rgb24, null, 0);

            modifiedBitmapSource.CopyPixels(pixelData, stride, 0);
            var contentData = new byte[stride * height ];
            for (int i = 0; i < pixelData.Length; i+=bytesPerPixel)
            {
                if (pixelData[i] % 2 == 0)
                {
                    for (int j = 0; j < bytesPerPixel; j++)
                    {
                        contentData[i + j] = 255;
                    }
                }
                else
                {
                    for (int j = 0; j < bytesPerPixel; j++)
                    {
                        contentData[i + j] = 0;
                    }
                }
                
            }
            var result = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, contentData, stride);
            return result;
        }

        public BitmapImage Hide(BitmapSource Content, BitmapImage OriginalMsg)
        {
            
        }
    }
}
