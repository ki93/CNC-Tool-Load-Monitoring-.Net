using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

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
        public SetProductInfoPeriod(ref OpWindow opWindow, DateTime startTime, DateTime endTime)
        {
            InitializeComponent();
            _opWindow = opWindow;
            ProductInfoStartDatePick.SelectedDate = startTime;
            ProductInfoStartDatePick.DisplayDateEnd = DateTime.Today;
            //추후 수정
            ProductInfoStartDatePick.DisplayDateStart = DateTime.Today.AddDays(-31);
            ProductInfoEndDatePick.SelectedDate = endTime;
            ProductInfoEndDatePick.DisplayDateEnd = DateTime.Today;
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
            _startTime = (DateTime)ProductInfoStartDatePick.SelectedDate;
            ProductInfoEndDatePick.DisplayDateStart = ProductInfoStartDatePick.SelectedDate;
        }

        //end 달력 선택 이벤트
        private void InfoEndDatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _endTime = (DateTime)ProductInfoEndDatePick.SelectedDate;
        }
    }
}
