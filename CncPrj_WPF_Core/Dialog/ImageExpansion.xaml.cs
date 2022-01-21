using System;
using System.Windows;
using System.Windows.Media.Imaging;


namespace CncPrj_WPF_Core
{
    /// <summary>
    /// ImageExpansion.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageExpansion : Window
    {
        public ImageExpansion(string fftSource, string judgeSn, string judgeResult)
        {
            InitializeComponent();
            fftImg.Source = new BitmapImage(new Uri(fftSource, UriKind.Absolute));
            ProductQuiltyInfo.Content = judgeSn + " : " + judgeResult;
        }

        private void CloseExpansionWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
