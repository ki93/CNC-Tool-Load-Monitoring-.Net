using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// historyIntervalSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetHistoryChartPeriod : Window
    {
        public String startTimeValue;
        public String endTimeValue;
        public OpWindow opwin;

        public SetHistoryChartPeriod(ref OpWindow opWindow, DateTime hitoryStartTime, DateTime hitoryEndTime)
        {
            InitializeComponent();
            SetHistoryChartPeriod setHistory = this;
            opwin = opWindow;
            HitoryStartDatePick.SelectedDate = hitoryStartTime;
            HitoryEndDatePick.SelectedDate = hitoryEndTime;
            HitoryStartDatePick.DisplayDateEnd = DateTime.Today;
            HitoryEndDatePick.DisplayDateEnd = DateTime.Today;
        }

        private void ClosehistorySettingWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //relative time range  설정 버튼 (5min 10min 15min btn)누를 때 이벤트
        private void SetChartHistoryTimeBtn(object sender, RoutedEventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var selectBtn = (Brush)converter.ConvertFromString("#737373");
            var unselectBtn = (Brush)converter.ConvertFromString("#1e1e1e");

            HitoryStartDatePick.SelectedDate = DateTime.Today;
            HitoryEndDatePick.SelectedDate = DateTime.Today;

            if (sender.Equals(Set5minBtn))
            {
                Set5minBtn.Background = selectBtn;
                Set10minBtn.Background = unselectBtn;
                Set15minBtn.Background = unselectBtn;
                startTimeValue = DateTime.Now.AddMinutes(-5).ToString();
                endTimeValue = DateTime.Now.ToString();
            }
            else if (sender.Equals(Set10minBtn))
            {
                Set5minBtn.Background = unselectBtn;
                Set10minBtn.Background = selectBtn;
                Set15minBtn.Background = unselectBtn;
                startTimeValue = DateTime.Now.AddMinutes(-10).ToString();
                endTimeValue = DateTime.Now.ToString();
            }
            else if (sender.Equals(Set15minBtn))
            {
                Set5minBtn.Background = unselectBtn;
                Set10minBtn.Background = unselectBtn;
                Set15minBtn.Background = selectBtn;
                startTimeValue = DateTime.Now.AddMinutes(-15).ToString();
                endTimeValue = DateTime.Now.ToString();
            }
        }
        //공정 페이지에 시작, 끝 점 넘기는
        public void SetPeriodValue(string startTime, string endTime)
        {
            opwin.setTimeRange(startTime, endTime);
        }
        //apply btn click evt
        private void SubmitSettingChartHistory_Click(object sender, RoutedEventArgs e)
        {
            Close();
            SetPeriodValue(startTimeValue, endTimeValue);
        }
        //start 달력 선택 이벤트
        private void HitoryStartDatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var unselectBtn = (Brush)converter.ConvertFromString("#1e1e1e");

            startTimeValue = HitoryStartDatePick.SelectedDate.ToString();
            HitoryEndDatePick.DisplayDateStart = HitoryStartDatePick.SelectedDate;
            HitoryEndDatePick.DisplayDateEnd = DateTime.Parse(HitoryStartDatePick.SelectedDate.ToString()).AddDays(30);

            if (HitoryEndDatePick.DisplayDateEnd > DateTime.Today)
            {
                HitoryEndDatePick.DisplayDateEnd = DateTime.Today;
            }

            //DateTime dateTime = DateTime.Parse(HitoryStartDatePick.SelectedDate.ToString()).ToLocalTime();
            //startTimeValue = dateTime.AddHours(-12).ToString();
            //var startPick = HitoryStartDatePick.SelectedDate;
            //var endPickShow = DateTime.Parse(HitoryStartDatePick.SelectedDate.ToString()).AddDays(30);

            Set5minBtn.Background = unselectBtn;
            Set10minBtn.Background = unselectBtn;
            Set15minBtn.Background = unselectBtn;

        }
        //end 달력 선택 이벤트
        private void HitoryEndDatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var unselectBtn = (Brush)converter.ConvertFromString("#1e1e1e");

            //endTimeValue = HitoryEndDatePick.SelectedDate.ToString();
           
            DateTime endDateTime = DateTime.Parse(HitoryEndDatePick.SelectedDate.ToString()).ToLocalTime();

            var currTime = Convert.ToDateTime(DateTime.Now.ToString("MM-dd"));
            var pickTime = Convert.ToDateTime(endDateTime.ToString("MM-dd"));

            if (DateTime.Compare(currTime, pickTime) == 0)
            {
                endTimeValue = DateTime.Now.ToString();
                Debug.WriteLine(endTimeValue);
            }
            else
            {
                endTimeValue = endDateTime.AddHours(14).AddMinutes(59).AddSeconds(59).ToString();
            }
            Set5minBtn.Background = unselectBtn;
            Set10minBtn.Background = unselectBtn;
            Set15minBtn.Background = unselectBtn;

        }
    }
}
