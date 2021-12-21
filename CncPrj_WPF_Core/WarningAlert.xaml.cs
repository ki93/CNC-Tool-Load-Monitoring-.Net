using System.Windows;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// WarningAlert.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WarningAlert : Window
    {
        public WarningAlert(string message)
        {
            InitializeComponent();
            warnMsg.Text = message;
        }

        private void WarningAlertClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
