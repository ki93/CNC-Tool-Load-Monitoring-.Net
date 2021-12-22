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
        public string startTime;
        public string endTime;
        public SetProductInfoPeriod()
        {
            InitializeComponent();
            ProductInfoStartDatePick.SelectedDate = DateTime.Today;
            ProductInfoEndDatePick.SelectedDate = DateTime.Today;
        }

        //Apply 버튼 이벤트 
        private void SubmitSetPrInfo_Click(object sender, RoutedEventArgs e)
        {
            //설정한 기간 전송
            Debug.WriteLine(startTime, endTime);
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //start 달력 선택 이벤트
        private void InfoStartDatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startTime = ProductInfoStartDatePick.SelectedDate.ToString();
            ProductInfoEndDatePick.DisplayDateStart = ProductInfoStartDatePick.SelectedDate;
            //일단 30일까지로 잡음. (논의 후 수정 예정)
            ProductInfoEndDatePick.DisplayDateEnd = DateTime.Parse(ProductInfoStartDatePick.SelectedDate.ToString()).AddDays(30);

            if (ProductInfoEndDatePick.DisplayDateEnd > DateTime.Today)
            {
                ProductInfoEndDatePick.DisplayDateEnd = DateTime.Today;
            }
        }

        //end 달력 선택 이벤트
        private void InfoEndDatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endTime = ProductInfoEndDatePick.SelectedDate.ToString();
        }
    }
}
