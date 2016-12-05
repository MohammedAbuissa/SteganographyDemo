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
    class PngSteganography : ISteganography<BitmapSource, BitmapSource>
    {

        ISteganography<byte[,], bool[,]> RawImageSteganography = new ImageSteganography();

        public BitmapSource Find(BitmapSource ModifiedMsg)
        {
            if (ModifiedMsg == null)
                throw new ArgumentNullException($"{nameof(ModifiedMsg)} can't be null");
            var height = ModifiedMsg.PixelHeight;
            var width = ModifiedMsg.PixelWidth;
            var bytesPerPixel = 3;
            var stride = width * bytesPerPixel;

            var pixelData = BitmapToByte(ModifiedMsg);

            var redData = ExtractRedChannel(pixelData, width, height, stride);
            var bWData = RawImageSteganography.Find(redData);

            return BoolToBitmap(bWData, height, width, stride);
        }

        BitmapSource BoolToBitmap(bool[,] BWData, int PixelHeight, int PixelWidth, int Stride)
        {
            var bytesPerPixel = Stride / PixelWidth;
            var contentData = new byte[Stride * PixelHeight];
            for (int i = 0; i < BWData.GetLength(0); i++)
            {
                for (int j = 0; j < BWData.GetLength(1); j++)
                {
                    var contentDataIterator = i * Stride + j * bytesPerPixel;
                    if (BWData[i, j])
                    {
                        for (int k = 0; k < bytesPerPixel; k++)
                        {
                            contentData[contentDataIterator + k] = 255;
                        }
                    }
                    else
                    {
                        for (int k = 0; k < bytesPerPixel; k++)
                        {
                            contentData[contentDataIterator + k] = 0;
                        }
                    }
                }
            }
            return BitmapSource.Create(PixelWidth, PixelHeight, 96, 96, PixelFormats.Rgb24, null, contentData, Stride);
        }

        byte[] BitmapToByte (BitmapSource Image)
        {
            var height = Image.PixelHeight;
            var width = Image.PixelWidth;
            var bytesPerPixel = 3;
            var stride = width * bytesPerPixel;

            if (Image.Format != PixelFormats.Rgb24)
                Image = new FormatConvertedBitmap(Image, PixelFormats.Rgb24, null, 0);

            var pixelData = new byte[stride * height];
            Image.CopyPixels(pixelData, stride, 0);
            return pixelData;
        }

        byte[,] ExtractRedChannel(byte[] ImageData, int PixelWidth, int PixelHeight, int Stride)
        {
            var redData = new byte[PixelHeight, PixelWidth];
            var bytesPerPixel = Stride / PixelWidth;
            for (int i = 0; i < redData.GetLength(0); i++)
            {
                for (int j = 0; j < redData.GetLength(1); j++)
                {
                    redData[i, j] = ImageData[i * Stride + j * bytesPerPixel];
                }
            }
            return redData;
        }

        bool[,] ByteToBool (byte[,] ByteData)
        {
            bool[,] boolData = new bool[ByteData.GetLength(0), ByteData.GetLength(1)];
            for (int i = 0; i < ByteData.GetLength(0); i++)
            {
                for (int j = 0; j < ByteData.GetLength(1); j++)
                {
                    boolData[i, j] = ByteData[i, j] > byte.MaxValue / 2;
                }
            }
            return boolData;
        }

        byte[] InsertRedChannel (byte[,] RedData, byte[] OriginalData, int Stride, int BytesPerPixel)
        {
            var originalClone = (byte[]) OriginalData.Clone();
            for (int i = 0; i < RedData.GetLength(0); i++)
            {
                for (int j = 0; j < RedData.GetLength(1); j++)
                {
                    originalClone[i * Stride + j * BytesPerPixel] = RedData[i, j];
                }
            }
            return originalClone;
        } 

        public BitmapSource Hide(BitmapSource Content, BitmapSource OriginalMsg)
        {
            if (Content == null)
                throw new ArgumentNullException($"{nameof(Content)} can't be null");
            if (OriginalMsg == null)
                throw new ArgumentNullException($"{nameof(OriginalMsg)} can't be null");
            var height = OriginalMsg.PixelHeight;
            var width = OriginalMsg.PixelWidth;
            var bytesPerPixel = 3;
            var stride = width * bytesPerPixel;

            var originalData = BitmapToByte(OriginalMsg);
            var originalRedData = ExtractRedChannel(originalData, width, height, stride);

            var contentData = BitmapToByte(Content);
            var contentRedData = ExtractRedChannel(contentData, width, height, stride);
            var contentBoolData = ByteToBool(contentRedData);

            var modifiedRedData = RawImageSteganography.Hide(contentBoolData, originalRedData);
            var modifiedData = InsertRedChannel(modifiedRedData, originalData, stride, bytesPerPixel);

            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, modifiedData, stride);

        }

    }
}
