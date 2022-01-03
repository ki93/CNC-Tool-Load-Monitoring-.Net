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
        public Login _login;
        public OpWindow _opWindow;
        public Dictionary<Object, Object> _alerts;
        string _id;

        public MoveStep()
        {
            InitializeComponent();
            timer = new DispatcherTimer(); //호출 함수 설정
            timer.Tick += timer_Tick; //함수 호출 주기 설정
            timer.Interval = TimeSpan.FromSeconds(1); //타이머 시작
            timer.Start();
            _alerts = new Dictionary<object, object>();
        }
        public void NavigationServiceLoadCompleted(object sender, NavigationEventArgs e)
        {
            _id = e.ExtraData.ToString();
            userId.Text = _id;
            NavigationService.LoadCompleted -= NavigationServiceLoadCompleted;
        }

        private void logoutEvt(object sender, RoutedEventArgs e)
        {
            _login._moveStep = this;
            NavigationService.LoadCompleted += _login.NavigationServiceLoadCompleted;
            NavigationService.Navigate(_login);
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            currentTime.Text = System.DateTime.Now.ToString("yyyy-MM-dd ddd HH:mm:ss");
        }

        private void MoveOp1(object sender, RoutedEventArgs e)
        {
            _opWindow = _login._opWindow;
            NavigationService.LoadCompleted += _opWindow.NavigationServiceLoadCompleted;
            NavigationService.Navigate(_opWindow, _id);
        }
    }
}
