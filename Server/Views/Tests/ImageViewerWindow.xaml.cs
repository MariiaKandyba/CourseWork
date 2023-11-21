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
            grid.Children.Add(dynamicImage);
        }

        private Image CreateDynamicImage(byte[] imageData)
        {
            Image dynamicImage = new ();
            BitmapImage bitmapImage = new();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageData);
            bitmapImage.EndInit();
            dynamicImage.Source = bitmapImage;
            dynamicImage.Stretch = Stretch.Uniform; 

            return dynamicImage;
        }

}
}
