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
        string _errorMessageTitle;
        string _errorMessage;

        public ErrorAlert(string title, string message)
        {
            InitializeComponent();
            _errorMessageTitle = title;
            _errorMessage = message;
            uErrorMessageTitle.Text = _errorMessageTitle;
            uErrorMessage.Text = _errorMessage;
            _count = 0;
        }
        public string GetErrorMessageTitle()
        {
            return _errorMessageTitle;
        }
        public string GetErrorMessage()
        {
            return _errorMessage;
        }

        public void CountUp()
        {
            _count++;
            uAlertCount.Text = $"(+{_count})";
        }

        private void ErrorAlertClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
