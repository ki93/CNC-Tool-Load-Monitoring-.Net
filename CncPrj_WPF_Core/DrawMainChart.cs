using System;
using SciChart.Data.Model;
using SciChart.Charting.Model.DataSeries;
using HNInc.Communication.Library;
using System.Collections.Generic;
using SciChart.Charting.Visuals.Axes;

namespace CncPrj_WPF_Core
{
    public class DrawMainChart
    {
        private OpWindow opwindow;
        private XyDataSeries<DateTime, Double> realSpindleLoad;
        private XyDataSeries<DateTime, Double> predictedSpindleLoad;
        private XyDataSeries<DateTime, Double> mae;

        private XyDataSeries<DateTime, Double> historyScaleLoad;
        private XyDataSeries<DateTime, Double> historyScalePredict;
        private XyDataSeries<DateTime, Double> historyMae;

        DateTime currentDateTime = new DateTime();
        DateTime currentHistoryDateTime = new DateTime();

        public DrawMainChart(ref OpWindow opwin)
        {
            opwindow = opwin;
            realSpindleLoad = new XyDataSeries<DateTime, Double>() { SeriesName = "RealTime Spindle Load" };
            predictedSpindleLoad = new XyDataSeries<DateTime, Double>() { SeriesName = "Predicted Spindle Load" };
            mae = new XyDataSeries<DateTime, Double>() { SeriesName = "Mae" };

            historyScaleLoad = new XyDataSeries<DateTime, Double>() { SeriesName = "Spindle Load" };
            historyScalePredict = new XyDataSeries<DateTime, Double>() { SeriesName = "Predicted Spindle Load" };
            historyMae = new XyDataSeries<DateTime, Double>() { SeriesName = "Mae" };
        }
  
        //main chart data append
        public void RealtimeChart(object data)
        {
            List<SocketRealTimeLoad> realTimeLoad = (List<SocketRealTimeLoad>)data;
            foreach (SocketRealTimeLoad streamPredict in realTimeLoad)
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                DateTime date = origin.AddMilliseconds(Double.Parse(streamPredict._time)).ToLocalTime();
                Double scaleLoad = Double.Parse(streamPredict._scaleLoad);
                Double scalePredict = Double.Parse(streamPredict._scalePredict);
                Double maeValue = Double.Parse(streamPredict._mae);
                if (currentDateTime < date) // time 값이 들어올 때 마다 커런트데이트타임으로 할당하고 그 값보다 더 작은 타임값이 들어오는건 어펜트 못하게
                {
                    currentDateTime = date;
                    realSpindleLoad.Append(currentDateTime, scaleLoad);
                    predictedSpindleLoad.Append(currentDateTime, scalePredict);
                    mae.Append(currentDateTime, maeValue);
                    opwindow.RealLoadLineSeries.DataSeries = realSpindleLoad;
                    opwindow.PredictLoadLineSeries.DataSeries = predictedSpindleLoad;
                    opwindow.RealMaeLineSeries.DataSeries = mae;
                    opwindow.realTimeLoadSpindleSciChartSurface.XAxis.VisibleRange = new DateRange(date.AddMinutes(-5), date);
                    opwindow.realTimeMaeSciChartSurface.XAxis.VisibleRange = new DateRange(date.AddMinutes(-5), date);
                }
                else
                {
                    //Debug.WriteLine("CurrentTime : {0}, InputDate : {1}", currentDateTime, date);
                }
            }
        }
        //history chart data append
        public void HistoryChart(List<HttpSpindleLoad> spindleLoads)
        {
            currentHistoryDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            historyScaleLoad.Clear();
            historyScalePredict.Clear();
            historyMae.Clear();

            foreach (HttpSpindleLoad spindleLoad in spindleLoads)
            {
                DateTime dateTime = DateTime.Parse(spindleLoad._time).ToLocalTime();
                string hitroyTime = dateTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                DateTime historydate = Convert.ToDateTime(hitroyTime);
                if (spindleLoad._meanMae != "null" && spindleLoad._meanScaleLoad != "null" && spindleLoad._meanScalePredict != "null")
                {
                    double meanScaleLoad = Convert.ToDouble(spindleLoad._meanScaleLoad);
                    double meanScalePredict = Convert.ToDouble(spindleLoad._meanScalePredict);
                    double meanMae = Convert.ToDouble(spindleLoad._meanMae);

                    if (currentHistoryDateTime < historydate)
                    {

                        currentHistoryDateTime = historydate;
                        historyScaleLoad.Append(currentHistoryDateTime, meanScaleLoad);
                        historyScalePredict.Append(currentHistoryDateTime, meanScalePredict);
                        historyMae.Append(currentHistoryDateTime, meanMae);
                        var pastTimeLoadSpindleYAxis = opwindow.pastTimeLoadSpindleSciChartSurface.YAxis;
                        var pastTimeLoadSpindleMaerange = new DoubleRange(0, 1);
                        pastTimeLoadSpindleYAxis.VisibleRangeChanged += (s, e) => pastTimeLoadSpindleYAxis.VisibleRange = pastTimeLoadSpindleMaerange;
                        var pastTimeMaeyAxis = opwindow.pastTimeMaeSciChartSurface.YAxis;
                        var pastTimeMaerange = new DoubleRange(0, 1);
                        pastTimeMaeyAxis.VisibleRangeChanged += (s,e) => pastTimeMaeyAxis.VisibleRange = pastTimeMaerange;
                        opwindow.PastLoadLineSeries.DataSeries = historyScaleLoad;
                        opwindow.PastPredictLoadLineSeries.DataSeries = historyScalePredict;
                        opwindow.PastMaeLineSeries.DataSeries = historyMae;
                    }
                    else
                    {
                        //Debug.WriteLine("ELSE currentHistoryDateTime : {0}, InputDate : {1}", currentHistoryDateTime, historydate);
                    }
                }
            }

        }
    }
}
