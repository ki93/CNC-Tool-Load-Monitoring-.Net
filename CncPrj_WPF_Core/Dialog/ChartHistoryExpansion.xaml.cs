using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using HNInc.Communication.Library;
using SciChart.Charting.Model.DataSeries;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// ChartHistoryExpansion.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChartHistoryExpansion : Window
    {
        public OpWindow opwin;
        HNHttp _hNHttp;
        private XyDataSeries<DateTime, Double> historyScaleLoad;
        private XyDataSeries<DateTime, Double> historyScalePredict;
        private XyDataSeries<DateTime, Double> historyMae;

        DateTime currentHistoryDateTime = new DateTime();
        public ChartHistoryExpansion(ref OpWindow opwindow)
        {
            InitializeComponent();
            opwin = opwindow;
            _hNHttp = new HNHttp(ConfigurationManager.AppSettings.Get("WasUrl"), ConfigurationManager.AppSettings.Get("AccountDBURL"), ConfigurationManager.AppSettings.Get("AccountDB"));
            historyScaleLoad = new XyDataSeries<DateTime, Double>() { SeriesName = "Spindle Load" };
            historyScalePredict = new XyDataSeries<DateTime, Double>() { SeriesName = "Predicted Spindle Load" };
            historyMae = new XyDataSeries<DateTime, Double>() { SeriesName = "Mae" };

        }

        private void CloseExpansionWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //http 연결
        public void chartExpansion(string startTime, string endTime, string groupBy)
        {
            List<HttpSpindleLoad> spindleLoads = _hNHttp.GetSpindleLoadList(Convert.ToDateTime(startTime).ToUniversalTime(), Convert.ToDateTime(endTime).ToUniversalTime(), HttpOPCode.OP10_3, groupBy);
            opwin.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                opwin.historyChartLoadBack.Visibility = Visibility.Hidden;
                opwin.historyChartLoadImg.Visibility = Visibility.Hidden;
                HistoryChart(spindleLoads);
                ShowDialog();
            }));
        }

        //draw Chart
        public void HistoryChart(List<HttpSpindleLoad> spindleLoads)
        {
            
            if (PastLoadLineSeries.DataSeries != null)
            {
                //currentHistoryDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                //historyScaleLoad.Clear();
                //historyScalePredict.Clear();
                //historyMae.Clear();
                //PastLoadLineSeries.DataSeries.Clear();
                //PastPredictLoadLineSeries.DataSeries.Clear();
                //PastMaeLineSeries.DataSeries.Clear();

            }
            foreach (HttpSpindleLoad spindleLoad in spindleLoads)
            {
                // UTC 타임으로 설정
                DateTime dateTime = DateTime.Parse(spindleLoad._time).ToLocalTime();
                //// tostring으로 변환하는 부분 
                string hitroyTime = dateTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                DateTime historydate = Convert.ToDateTime(hitroyTime);
                if (spindleLoad._meanMae != "null" && spindleLoad._meanScaleLoad != "null" && spindleLoad._meanScalePredict != "null")
                {
                    double meanScaleLoad = Convert.ToDouble(spindleLoad._meanScaleLoad);
                    double meanScalePredict = Convert.ToDouble(spindleLoad._meanScalePredict);
                    double meanMae = Convert.ToDouble(spindleLoad._meanMae);

                    if (currentHistoryDateTime < historydate)
                    {
                        //Debug.WriteLine("IF currentHistoryDateTime : {0}, InputDate : {1}", currentHistoryDateTime, historydate);
                        currentHistoryDateTime = historydate;
                        historyScaleLoad.Append(currentHistoryDateTime, meanScaleLoad);
                        historyScalePredict.Append(currentHistoryDateTime, meanScalePredict);
                        historyMae.Append(currentHistoryDateTime, meanMae);
                        PastLoadLineSeries.DataSeries = historyScaleLoad;
                        PastPredictLoadLineSeries.DataSeries = historyScalePredict;
                        PastMaeLineSeries.DataSeries = historyMae;
                    }
                    else
                    {
                    }
                }
            }

        }

    }
}