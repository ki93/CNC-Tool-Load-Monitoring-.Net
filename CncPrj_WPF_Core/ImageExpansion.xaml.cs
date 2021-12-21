using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace CncPrj_WPF_Core
{
    /// <summary>
    /// ImageExpansion.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageExpansion : Window
    {
        public ImageExpansion(string fftSource)
        {
            InitializeComponent();
            fftImg.Source = new BitmapImage(new Uri(fftSource, UriKind.Absolute));
        }

        private void CloseExpansionWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
