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
    /// ErrorAlert.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ErrorAlert : Window
    {
        int _count;
        OpWindow _opWindow;

        public ErrorAlert(string message, ref OpWindow opWindow)
        {
            InitializeComponent();
            errMsg.Text = message;
            _opWindow = opWindow;
            _count = 0;
        }
        public void CountUp()
        {
            _count++;
            uAlertCount.Text = $"(+{_count})";
        }

        private void ErrorAlertClose(object sender, RoutedEventArgs e)
        {
            _opWindow._alerts.Remove(errMsg.Text);
            Close();
        }
    }
}
