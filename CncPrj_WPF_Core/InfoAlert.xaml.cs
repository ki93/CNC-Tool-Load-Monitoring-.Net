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
using System.Windows.Shapes;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// InfoAlert.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InfoAlert : Window
    {

        public InfoAlert(string message)
        {
            InitializeComponent();
            infoMsg.Text = message;
        }

        private void InfoAlertClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
