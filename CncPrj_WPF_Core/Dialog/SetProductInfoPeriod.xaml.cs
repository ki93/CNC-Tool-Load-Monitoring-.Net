using CncPrj_WPF_Core.Alert;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// SetProductInfoPeriod.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetProductInfoPeriod : Window
    {
        DateTime _startTime;
        DateTime _endTime;
        OpWindow _opWindow;
        Alerts _alerts; 

        public SetProductInfoPeriod(ref OpWindow opWindow, DateTime startTime, DateTime endTime)
        {
            InitializeComponent();
            _opWindow = opWindow;
            _alerts = new Alerts();

            //추후 수정
            ProductInfoStartDatePick.DisplayDateStart = DateTime.Today.AddDays(-31);
            ProductInfoEndDatePick.SelectedDate = endTime;
            ProductInfoEndDatePick.DisplayDateEnd = DateTime.Today;
            ProductInfoStartDatePick.SelectedDate = startTime;
            ProductInfoStartDatePick.DisplayDateEnd = DateTime.Today;
        }

        //Apply 버튼 이벤트 
        private void SubmitSetPrInfo_Click(object sender, RoutedEventArgs e)
        {
            //설정한 기간 전송
            _opWindow.RequestProductInfoList(_startTime, _endTime);
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //start 달력 선택 이벤트
        private void InfoStartDatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime temp = (DateTime)ProductInfoStartDatePick.SelectedDate;
            ProductInfoEndDatePick.DisplayDateStart = ProductInfoStartDatePick.SelectedDate;
            if (_endTime < temp)
            {
                ProductInfoStartDatePick.SelectedDate = _startTime;
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        string message = "시작 날짜를 다시 설정해주세요. 종료 날짜보다 느릴 수 없습니다.";
                        _alerts.CreateAlert(AlertCategory.Information, sender.ToString(), message);
                    }));
                });
            }
            else
            {
                _startTime = temp;
            }
        }

        //end 달력 선택 이벤트
        private void InfoEndDatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _endTime = (DateTime)ProductInfoEndDatePick.SelectedDate;
        }
    }
}
