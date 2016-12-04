using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steganography;
using System.Windows.Media.Imaging;

namespace SteganographyDemo
{
    class PngSteganography : ISteganography<BitmapImage, BitmapImage>
    {

        ISteganography<byte[,], bool[,]> RawImageSteganography = new ImageSteganography();

        public BitmapImage Find(BitmapImage ModifiedMsg)
        {
            throw new NotImplementedException();
        }

        public BitmapImage Hide(BitmapImage Content, BitmapImage OriginalMsg)
        {
            throw new NotImplementedException();
        }
    }
}
