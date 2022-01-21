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
        string _warningMessageTitle;
        string _warningMessage;

        public WarningAlert(string title, string message)
        {
            InitializeComponent();
            _warningMessageTitle = title;
            _warningMessage = message;
            uWarningMessageTitle.Text = _warningMessageTitle;
            uWarningMessage.Text = _warningMessage;
            _count = 0;
        }
        public string GetWarningMessageTitle()
        {
            return _warningMessageTitle;
        }
        public string GetWarningMessage()
        {
            return _warningMessage;
        }

        public void CountUp()
        {
            _count++;
            uAlertCount.Text = $"(+{_count})";
        }
        private void WarningAlertClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
