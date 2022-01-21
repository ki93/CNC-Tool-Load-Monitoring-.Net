
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using HNInc.Communication.Library;
using SciChart.Charting.Model.DataSeries;


namespace CncPrj_WPF_Core
{
    /// <summary>
    /// ProductCycleChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProductCycleChart : Window
    {
        private XyDataSeries<DateTime, Double> productLoad;
        private XyDataSeries<DateTime, Double> productPredict;
        private XyDataSeries<DateTime, Double> productMae;

        public ProductCycleChart()
        {
            InitializeComponent();
            productLoad = new XyDataSeries<DateTime, double> { SeriesName = "Prodcut's Spindle Load" };
            productPredict = new XyDataSeries<DateTime, double> { SeriesName = "Prodcut's Predict Spindle Load" };
            productMae = new XyDataSeries<DateTime, double> { SeriesName = "Prodcut's Mae" };
        }

        //window close
        private void CloseExpansionWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //http conn
        public void httpConn(string opcode, string sn)
        {
            string parseOPcode = opcode.Replace("-","_");
            HttpOPCode oPCode = (HttpOPCode)Enum.Parse(typeof(HttpOPCode), parseOPcode);
            List<HttpCycleInformaiton> cycleInfo = HNHttp.GetCycleInformationList(oPCode, sn);
            InputProdcutCycleChart(cycleInfo);
            
        }

        //draw Chart
        public void InputProdcutCycleChart(List<HttpCycleInformaiton> cycleInfo)
        {
            foreach (HttpCycleInformaiton item in cycleInfo)
            {
                Debug.WriteLine(item);
                DateTime time = DateTime.Parse(item._time);
                Double sclaeLoad = Double.Parse(item._scaleLoad);
                Double sclaePredict = Double.Parse(item._scalePredict);
                Double mae = Double.Parse(item._mae);

                productLoad.Append(time, sclaeLoad);
                productPredict.Append(time, sclaePredict);
                productMae.Append(time, mae);

                ScaleLoadLineSeries.DataSeries = productLoad;
                ScalePredictLoadLineSeries.DataSeries = productPredict;
                ScaleMaeLineSeries.DataSeries = productMae;
            }
        }
    }
}
