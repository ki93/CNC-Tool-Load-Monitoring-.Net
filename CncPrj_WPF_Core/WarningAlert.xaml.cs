using System;
using System.Windows;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// WarningAlert.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WarningAlert : Window
    {
        int _count;
        OpWindow _opWindow;

        public WarningAlert(string message, ref OpWindow opWindow)
        {
            InitializeComponent();
            warnMsg.Text = message;
            _opWindow = opWindow;
            _count = 0;
        }
        public void CountUp()
        {
            _count++;
            uAlertCount.Text = $"(+{_count})";
        }
        private void WarningAlertClose(object sender, RoutedEventArgs e)
        {
            _opWindow._alerts.Remove(warnMsg.Text);
            Close();
        }
    }
}
