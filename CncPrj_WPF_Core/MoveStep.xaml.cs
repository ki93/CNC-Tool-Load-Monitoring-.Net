using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// MoveStep.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MoveStep : Page
    {
        DispatcherTimer timer;

        public MoveStep()
        {
            InitializeComponent();
            timer = new DispatcherTimer(); //호출 함수 설정
            timer.Tick += timer_Tick; //함수 호출 주기 설정
            timer.Interval = TimeSpan.FromSeconds(1); //타이머 시작
            timer.Start();
        }

        private void logoutEvt(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/Login.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            currentTime.Text = System.DateTime.Now.ToString("yyyy-MM-dd ddd HH:mm:ss");
        }

        private void MoveOp1(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/OpWindow.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}
