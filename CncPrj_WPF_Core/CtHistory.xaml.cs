using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Data;
using SciChart.Charting.Model.DataSeries;
using System.Diagnostics;
using HNInc.Communication.Library;
using SciChart.Charting.Visuals.Annotations;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// CtHistory.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CtHistory : Window
    {
        private DataTable CtDataTable;
        public CtHistory ctHistory;
        public string CtStartTime;
        public string CtEndTime;
        public TimeSpan CtCycleTime;

        public CtHistory()
        {
            InitializeComponent();
            ctHistory = this;
            CtDataTable = new DataTable();
            List<HttpCycleTime> cycleTimes = HNHttp.GetCycleTimeList(HttpOPCode.OP10_3, 100);
            List<HttpCycleTime> cycleTimesChart = HNHttp.GetCycleTimeList(HttpOPCode.OP10_3, 10);
            InputCycleTimeList(cycleTimes);
            InputBarChartData(cycleTimesChart);
        }

        //bar chart dataserise에 append
        private void InputBarChartData(List<HttpCycleTime> cycleTimesChart)
        {
            var count = 0;
            cycleTimesChart.Reverse();
            var ctHistoryDataSeries = new XyDataSeries<int, double> { SeriesName = "Cycle Time (Top 10)" };
            foreach (HttpCycleTime item in cycleTimesChart)
            {
                int ChartXValue = count + 1;
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                // UTC 타임으로 설정
                DateTime cycleTime = origin.AddMilliseconds(item._cycleTime);
                // tostring으로 변환하는 부분 
                var cycleMin = double.Parse(cycleTime.Minute.ToString()) * 60;
                var cycleSec = double.Parse(cycleTime.Second.ToString());
                Double ChartYValue = cycleMin + cycleSec;
                ctHistoryDataSeries.Append(ChartXValue, ChartYValue);

                var labelAnnotation = new TextAnnotation()
                {
                    FontSize = 15,
                    X1 = ChartXValue,
                    Y1 = ChartYValue,
                    Text = ChartYValue.ToString(),
                    //Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33)),
                    Foreground = new SolidColorBrush(Color.FromRgb(192, 192, 192)),
                    FontWeight = FontWeights.Bold,
                    VerticalAnchorPoint = VerticalAnchorPoint.Bottom,
                    HorizontalAnchorPoint = HorizontalAnchorPoint.Center
                };
                this.ctBarChart.Annotations.Add(labelAnnotation);
                count = ChartXValue;
            }
            this.ctHistorySeries.DataSeries = ctHistoryDataSeries;
        }

        //data grid 에 들어갈 데이터 받는 곳
        public void InputCycleTimeList(List<HttpCycleTime> cycleTimes)
        {
            CtDataTable.Columns.Add("CtStart", typeof(string));
            CtDataTable.Columns.Add("CtEnd", typeof(string));
            CtDataTable.Columns.Add("CtCount", typeof(string));

            foreach (HttpCycleTime item in cycleTimes)
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                // UTC 타임으로 설정
                DateTime startTime = origin.AddMilliseconds(Double.Parse(item._startTime)).ToLocalTime();
                // tostring으로 변환하는 부분
                CtStartTime = startTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                string cycleTime = item._transCycleTime;
                DateTime endTime = origin.AddMilliseconds(Double.Parse(item._endTime)).ToLocalTime();
                CtEndTime = endTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                CtDataTable.Rows.Add(CtStartTime, CtEndTime, cycleTime);
            }
        }

        //datagrid load
        private void CTdataGrid(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = CtDataTable.DefaultView;

        }
        //window close evt btn
        private void CloseCTWindow(object sender, RoutedEventArgs e)
        {
            Close();

        }
    }
}