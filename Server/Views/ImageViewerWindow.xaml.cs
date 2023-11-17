using iText.IO.Image;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Server.Views
{
    /// <summary>
    /// Interaction logic for ImageViewerWindow.xaml
    /// </summary>
    public partial class ImageViewerWindow : Window
    {
        public ImageViewerWindow(byte[] imageData)
        {
            InitializeComponent();
        Image dynamicImage = CreateDynamicImage(imageData);

        // Додати Image до вашого Grid
        grid.Children.Add(dynamicImage);
        }

        private Image CreateDynamicImage(byte[] imageData)
    {
        Image dynamicImage = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageData);
            bitmapImage.EndInit();

            // Присвоєння зображення в Image у новому вікні
            // Встановити властивості для зображення
            dynamicImage.Source = bitmapImage;
        dynamicImage.Stretch = Stretch.Uniform; // або інший тип Stretch, якщо потрібно

        // Ви можете також встановити інші властивості Image за необхідності

        return dynamicImage;
    }
    //public void SetImage(byte[] imageData)
    //{
    //    // Застосовуємо зображення до Image в новому вікні
    //    BitmapImage bitmapImage = new BitmapImage();
    //    bitmapImage.BeginInit();
    //    bitmapImage.StreamSource = new MemoryStream(imageData);
    //    bitmapImage.EndInit();

    //    // Присвоєння зображення в Image у новому вікні
    //    imageControl.Source = bitmapImage;
    //}
}
}
