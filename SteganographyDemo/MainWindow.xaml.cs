using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteganographyDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FindMsg_Click(object sender, RoutedEventArgs e)
        {
            var s = new PngSteganography();
            ContentImage.Source = s.Find(ModifiedImage.Source as BitmapImage);
        }

        private void HideMsg_Click(object sender, RoutedEventArgs e)
        {
            var s = new PngSteganography();
            ModifiedImage.Source = s.Hide(ContentImage.Source as BitmapSource, OriginalImage.Source as BitmapImage);
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.FileName = "Msg";
            openDialog.DefaultExt = ".png";
            openDialog.Filter = "Image files (*.png)| *.*";
            openDialog.FilterIndex = 0;
            if (openDialog.ShowDialog() == true)
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(openDialog.FileName, UriKind.Absolute);
                bitmapImage.EndInit();
                var buttonName = (sender as Button).Name;
                if (buttonName == "LoadModifiedMsg")
                    ModifiedImage.Source = bitmapImage;
                else if (buttonName == "LoadOriginalMsg")
                    OriginalImage.Source = bitmapImage;
                else if (buttonName == "LoadContent")
                    ContentImage.Source = bitmapImage;
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            var senderName = (sender as Button).Name;
            ImageSource targetImageSource = null;
            if (senderName == "SaveModifiedMsg")
                targetImageSource = ModifiedImage.Source;
            else if (senderName == "SaveContent")
                targetImageSource = ContentImage.Source;

            var saveDialogue = new SaveFileDialog();
            saveDialogue.FileName = "Msg";
            saveDialogue.DefaultExt = ".png";
            saveDialogue.Filter = "Image files (*.png)| *.*";
            saveDialogue.FilterIndex = 0;
            if (saveDialogue.ShowDialog() == true)
            {
                using (var stream = saveDialogue.OpenFile())
                {
                    if (stream != null)
                    {
                        var bitmap = targetImageSource as BitmapSource;
                        var encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        encoder.Save(stream);
                        MessageBox.Show("The image has been saved correctly");
                    }
                    else
                    {
                        MessageBox.Show("the stream equals null");
                    }
                }
            }
        }
    }
}
