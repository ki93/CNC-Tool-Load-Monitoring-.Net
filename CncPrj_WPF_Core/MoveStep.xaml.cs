using CncPrj_WPF_Core.Alert;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// MoveStep.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MoveStep : Page
    {
        DispatcherTimer timer;
        Alerts _alerts;
        string _id;


        public MoveStep()
        {
            InitializeComponent();
        }
        public void NavigationServiceLoadCompleted(object sender, NavigationEventArgs e)
        {
            timer = new DispatcherTimer(); //호출 함수 설정
            timer.Tick += timer_Tick; //함수 호출 주기 설정
            timer.Interval = TimeSpan.FromSeconds(0); //타이머 시작
            timer.Start();
            _alerts = new Alerts();

            _id = e.ExtraData.ToString();
            userId.Text = _id;
            NavigationService.LoadCompleted -= NavigationServiceLoadCompleted;
        }

        private void logoutEvt(object sender, RoutedEventArgs e)
        {
            timer.Tick -= timer_Tick;
            timer.Stop();
            timer = null;
            _alerts = null;
            Login login = new Login();
            NavigationService.LoadCompleted += login.NavigationServiceLoadCompleted;
            NavigationService.Navigate(login);
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            currentTime.Text = System.DateTime.Now.ToString("yyyy-MM-dd ddd HH:mm:ss");
        }

        private void MoveOp1(object sender, RoutedEventArgs e)
        {
            OpWindow opWindow = new OpWindow();
            NavigationService.LoadCompleted += opWindow.NavigationServiceLoadCompleted;
            NavigationService.Navigate(opWindow, _id);

        }
    }
}
