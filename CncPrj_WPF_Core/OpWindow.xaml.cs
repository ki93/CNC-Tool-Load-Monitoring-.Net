﻿using HNInc.Communication.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using SciChart.Charting.Model.DataSeries;
using System.IO;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using System.Windows.Input;
using System.Threading.Tasks;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// OpWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OpWindow : Page
    {
        DrawMainChart drawMainChart;
        //SetHistoryChartPeriod SetHistory;
        string hitoryStartTime;
        string hitoryEndTime;
        string hitoryGroupByTime;
        string fftSource;
        SocketConnecting socketConnecting;
        public OpWindow opwindow;
        DispatcherTimer timer;
        BrushConverter converter = new BrushConverter();
        bool maeFlag = true;
        private DataTable processTable;
        private Login userLogin;
        bool gridVisibility;

        public OpWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer(); //호출 함수 설정
            timer.Tick += timer_Tick; //함수 호출 주기 설정
            timer.Interval = TimeSpan.FromSeconds(1); //타이머 시작
            timer.Start();
            opwindow = this;
            drawMainChart = new DrawMainChart(ref opwindow);
            socketConnecting = new SocketConnecting(ref opwindow);
            socketConnecting.SocketConn(); //socket 연결
            var historyChartGroupByItem = (ComboBoxItem)historyChartGroupByValue.SelectedItem;
            hitoryGroupByTime = (string)historyChartGroupByItem.Content;
            hitoryStartTime = DateTime.Now.AddMinutes(-15).ToString();
            hitoryEndTime = DateTime.Now.ToString();

            historyChartInit();
            processTable = new DataTable();
            InputCycleTimeAverage();
            InputRealTimeCount();
            new JudgeQuality(ref opwindow).InitJudgeQualityImage();
        }

        public OpWindow(ref Login login)
        {
            InitializeComponent();
            userLogin = login;
        }

        public void InputUserId(string id)
        {
           userId.Text = id;
        }

        //mae 출력
        public void InputRealTimeLossData(string RTLossData, object data)
        {
            // drawMainChart스레드를 접근하기 위해서
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                SocketRealTimeLoss realTimeLoss = (SocketRealTimeLoss)(data);
                Double lossVal = (Double.Parse(realTimeLoss._maeData));
                string lossValue = string.Format("{0:F4}", lossVal);
                string anomalVal = realTimeLoss._anomalyData;

                string lossValResult = lossValue + "," + anomalVal;
                LinearGradientBrush normalBack = new LinearGradientBrush();
                normalBack.StartPoint = new Point(0, 0);
                normalBack.EndPoint = new Point(0, 1);
                normalBack.GradientStops.Add(new GradientStop(Color.FromRgb(56, 66, 54), 1.0));
                normalBack.GradientStops.Add(new GradientStop(Color.FromRgb(70, 89, 66), 0.0));

                LinearGradientBrush abnormalBack = new LinearGradientBrush();
                abnormalBack.StartPoint = new Point(0, 0);
                abnormalBack.EndPoint = new Point(0, 1);
                abnormalBack.GradientStops.Add(new GradientStop(Color.FromRgb(150, 18, 33), 1.0));
                abnormalBack.GradientStops.Add(new GradientStop(Color.FromRgb(196, 22, 42), 0.0));

                LinearGradientBrush alarmBack = new LinearGradientBrush();
                alarmBack.StartPoint = new Point(0, 0);
                alarmBack.EndPoint = new Point(0, 1);
                alarmBack.GradientStops.Add(new GradientStop(Color.FromRgb(196, 22, 42), 1.0));
                alarmBack.GradientStops.Add(new GradientStop(Color.FromRgb(255, 31, 56), 0.0));

                if (anomalVal == "정상")
                {
                    anomalVal = "Normal";
                    maeState.Fill = normalBack;
                    lossValResult = lossValue + "," + anomalVal;
                }
                else if (anomalVal == "비정상")
                {
                    anomalVal = "Abnormal";
                    if (maeFlag)
                    {
                        maeState.Fill = abnormalBack;
                        maeFlag = false;
                    }
                    else
                    {
                        maeState.Fill = alarmBack;
                        maeFlag = true;
                    }
                    lossValResult = lossValue + "," + anomalVal;
                }
                else if (anomalVal == "판정 중")
                {
                    anomalVal = "Predicting..";
                    lossValResult = anomalVal;
                }
                loss1sVal.Content = lossValResult;
            }));
        }
        //socket을 통해 받은 RealTime Load Data를 실시간 차트 그리는 곳으로 넘김
        public void InputRealTimeLoadData(string RTLoadData, object data)
        {
            // drawMainChart스레드를 접근하기 위해서
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                drawMainChart.RealtimeChart(data);
            }));
        }
        //RealTime product count 데이터 출력
        public void InputRealTimeCount()
        {
            HttpRealTimeCount realTimeCount = HNHttp.GetRealTimeCountRequest(HttpOPCode.OP10_3);
            
            Debug.WriteLine(realTimeCount.ToString());
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                totalproductVal.Content = realTimeCount._count;
            }));
        }
        //cycle time 5개 평균 데이터 출력
        public void InputCycleTimeAverage()
        {
            HttpCycleTimeAverage cycleTimeAverage = HNHttp.GetCycleTimeAverageRequest(5);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(cycleTimeAverage._mean);
                cycleTimeVal.Content = $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            }));
        }
        //가동 상태 데이터 출력
        public void InputDeiviceHealthCheck()
        {
            HttpDeiviceHealthCheck deiviceHealthCheck = HNHttp.GetDeiviceHealthCheckRequest(HttpOPCode.OP10_3);
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                if (deiviceHealthCheck._status == "success")
                {
                    LinearGradientBrush normalBack = new LinearGradientBrush();
                    normalBack.StartPoint = new Point(0, 0);
                    normalBack.EndPoint = new Point(0, 1);
                    normalBack.GradientStops.Add(new GradientStop(Color.FromRgb(56, 66, 54), 1.0));
                    normalBack.GradientStops.Add(new GradientStop(Color.FromRgb(70, 89, 66), 0.0));

                    opsituationState.Content = "Running";
                    //opsituationState.Background = normalBack;
                    runState.Fill = normalBack;
                }
                else
                {
                    LinearGradientBrush abnormalBack = new LinearGradientBrush();
                    abnormalBack.StartPoint = new Point(0, 0);
                    abnormalBack.EndPoint = new Point(0, 1);
                    abnormalBack.GradientStops.Add(new GradientStop(Color.FromRgb(150, 18, 33), 1.0));
                    abnormalBack.GradientStops.Add(new GradientStop(Color.FromRgb(196, 22, 42), 0.0));

                    opsituationState.Content = "Stop";
                    runState.Fill = abnormalBack;
                }
            }));
        }

        //상단 시간 출력 및 가동 상태 체크
        public void timer_Tick(object sender, EventArgs e)
        {
            currentTime.Text = System.DateTime.Now.ToString("yyyy-MM-dd ddd HH:mm:ss");
            InputDeiviceHealthCheck();
        }
        //total Procuct 새 윈도우 오픈
        private void tpHistoryEvt(object sender, RoutedEventArgs e)
        {
            TpHistory tpHistory = new TpHistory();
            tpHistory.ShowDialog();
        }
        //cycle time 새 윈도우 오픈
        public void ctHistory(object sender, RoutedEventArgs e)
        {
            CtHistory ctHistory = new CtHistory();
            ctHistory.ShowDialog(); //showdialog => 모달, 해당 창 닫기 전 까지 뒤 화면 이동 불가, show => 모달리스, 뒤 화면 제어, 이동 가능
        }
        //logout 버튼 이벤트
        private void logoutEvt(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/Login.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }
        //차트 이력 기간 설정 버튼 이벤트 -> 기간 설정 윈도우 오픈
        private void setPeriodBtn_Click(object sender, RoutedEventArgs e)
        {
            SetHistoryChartPeriod history = new SetHistoryChartPeriod(ref opwindow);
            history.ShowDialog();
        }
        //차트 history 기간
        public void setTimeRange(string startTime, string endTime)
        {
            hitoryStartTime = startTime;
            hitoryEndTime = endTime;
            //data http로 보내기
            HistoryChartHttpRequest();


            var timeRange = Convert.ToDateTime(endTime) - Convert.ToDateTime(startTime);

            if (timeRange.Minutes == 5)
            {
                setPeriodBtn.Content = "Last 5 minutes";
                combo1s.IsEnabled = true;
                combo2s.IsEnabled = false;
                combo3s.IsEnabled = false;
                combo4s.IsEnabled = false;
                combo5s.IsEnabled = false;
                combo6s.IsEnabled = false;
                combo7s.IsEnabled = false;
                combo8s.IsEnabled = false;
                combo9s.IsEnabled = false;
                combo10s.IsEnabled = false;
                historyChartGroupByValue.SelectedIndex = 0;
            }
            else if (timeRange.Minutes == 10)
            {
                setPeriodBtn.Content = "Last 10 minutes";
                combo1s.IsEnabled = true;
                combo2s.IsEnabled = true;
                combo3s.IsEnabled = true;
                combo4s.IsEnabled = false;
                combo5s.IsEnabled = false;
                combo6s.IsEnabled = false;
                combo7s.IsEnabled = false;
                combo8s.IsEnabled = false;
                combo9s.IsEnabled = false;
                combo10s.IsEnabled = false;
                historyChartGroupByValue.SelectedIndex = 2;
            }
            else if (timeRange.Minutes == 15)
            {
                setPeriodBtn.Content = "Last 15 minutes";
                combo1s.IsEnabled = true;
                combo2s.IsEnabled = true;
                combo3s.IsEnabled = true;
                combo4s.IsEnabled = false;
                combo5s.IsEnabled = false;
                combo6s.IsEnabled = false;
                combo7s.IsEnabled = false;
                combo8s.IsEnabled = false;
                combo9s.IsEnabled = false;
                combo10s.IsEnabled = false;
                historyChartGroupByValue.SelectedIndex = 2;
            }
            else if (timeRange.Minutes > 15 && timeRange.Days < 5)
            {
                setPeriodBtn.Content = startTime.Substring(0, 10) + " ~ " + endTime.Substring(0, 10);
                combo1s.IsEnabled = true;
                combo2s.IsEnabled = true;
                combo3s.IsEnabled = true;
                combo4s.IsEnabled = true;
                combo5s.IsEnabled = true;
                combo6s.IsEnabled = true;
                combo7s.IsEnabled = true;
                combo8s.IsEnabled = true;
                combo9s.IsEnabled = true;
                combo10s.IsEnabled = true;
                historyChartGroupByValue.SelectedIndex = 2;
            }
            else if (timeRange.Days >= 5 && timeRange.Days < 7)
            {
                combo1s.IsEnabled = false;
                combo2s.IsEnabled = true;
                combo3s.IsEnabled = true;
                combo4s.IsEnabled = true;
                combo5s.IsEnabled = true;
                combo6s.IsEnabled = true;
                combo7s.IsEnabled = true;
                combo8s.IsEnabled = true;
                combo9s.IsEnabled = true;
                combo10s.IsEnabled = true;
                historyChartGroupByValue.SelectedIndex = 2;
                setPeriodBtn.Content = startTime.Substring(0, 10) + " ~ " + endTime.Substring(0, 10);
            }
            else if (timeRange.Days >= 7 && timeRange.Days < 10)
            {
                combo1s.IsEnabled = false;
                combo2s.IsEnabled = false;
                combo3s.IsEnabled = true;
                combo4s.IsEnabled = true;
                combo5s.IsEnabled = true;
                combo6s.IsEnabled = true;
                combo7s.IsEnabled = true;
                combo8s.IsEnabled = true;
                combo9s.IsEnabled = true;
                combo10s.IsEnabled = true;
                historyChartGroupByValue.SelectedIndex = 2;
                setPeriodBtn.Content = startTime.Substring(0, 10) + " ~ " + endTime.Substring(0, 10);
            }
            else if (timeRange.Days >= 10 && timeRange.Days <= 30)
            {
                combo1s.IsEnabled = false;
                combo2s.IsEnabled = false;
                combo3s.IsEnabled = false;
                combo4s.IsEnabled = false;
                combo5s.IsEnabled = false;
                combo6s.IsEnabled = false;
                combo7s.IsEnabled = false;
                combo8s.IsEnabled = false;
                combo9s.IsEnabled = false;
                combo10s.IsEnabled = true;
                historyChartGroupByValue.SelectedIndex = 9;
                setPeriodBtn.Content = startTime.Substring(0, 10) + " ~ " + endTime.Substring(0, 10);
            }
        }
        //chart history expansion
        public void ChartExpansion(object sender, RoutedEventArgs e)
        {
            historyChartLoadMsg.Visibility = Visibility.Visible;
            Debug.WriteLine(DateTime.Now.ToLongTimeString());

            //새 윈도우로 데이터 넘기기   
            ChartHistoryExpansion expansion = new ChartHistoryExpansion(ref opwindow);
            Task.Run(() =>
            {
                expansion.chartExpansion(hitoryStartTime, hitoryEndTime, hitoryGroupByTime);
                //historyChartLoadMsg.Visibility = Visibility.Hidden;
                //historyChartLoadMsg.Visibility = Visibility.Hidden;

            });
        }
        //chart history combobox change evt
        private void historyChartGroupByValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var historyChartGroupByItem = (ComboBoxItem)historyChartGroupByValue.SelectedItem;
            hitoryGroupByTime = (string)historyChartGroupByItem.Content;
            //data http로 보내기
            HistoryChartHttpRequest();
        }
        //history chart 초기화
        public void historyChartInit()
        {
            hitoryStartTime = DateTime.Now.AddMinutes(-15).ToString();
            hitoryEndTime = DateTime.Now.ToString();
            hitoryGroupByTime = "3s";
            setPeriodBtn.Content = "Last 15 minutes";
            historyChartGroupByValue.SelectedIndex = 2;
            combo1s.IsEnabled = true;
            combo2s.IsEnabled = true;
            combo3s.IsEnabled = true;
            combo4s.IsEnabled = false;
            combo5s.IsEnabled = false;
            combo6s.IsEnabled = false;
            combo7s.IsEnabled = false;
            combo8s.IsEnabled = false;
            combo9s.IsEnabled = false;
            combo10s.IsEnabled = false;
            //data http로 보내기
            HistoryChartHttpRequest();
        }
        //chart history refresh
        private void RefreshHistoryChart(object sender, RoutedEventArgs e)
        {
            historyChartInit();
        }
        //history chart http 요청 보내기
        public void HistoryChartHttpRequest()
        {
            //data http로 보내기
            if (hitoryStartTime != null)
            {
                historyChartLoadMsg.Visibility = Visibility.Visible;

                Debug.WriteLine(hitoryStartTime, hitoryEndTime);
                //List<SpindleLoad> spindleLoads = HNHttp.GetSpindleLoadRequest((int)Convert.ToDateTime(hitoryStartTime).Year, (int)Convert.ToDateTime(hitoryStartTime).Month, (int)Convert.ToDateTime(hitoryStartTime).Day, (int)Convert.ToDateTime(hitoryEndTime).Year, (int)Convert.ToDateTime(hitoryEndTime).Month, (int)Convert.ToDateTime(hitoryEndTime).Day, OPCode.OP10_3, hitoryGroupByTime);
                Task.Run(() =>
                {
                    List<HttpSpindleLoad> spindleLoads = HNHttp.GetSpindleLoadRequest(Convert.ToDateTime(hitoryStartTime).ToUniversalTime(), Convert.ToDateTime(hitoryEndTime).ToUniversalTime(), HttpOPCode.OP10_3, hitoryGroupByTime);
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        drawMainChart.HistoryChart(spindleLoads);
                        historyChartLoadMsg.Visibility = Visibility.Hidden;
                    }));
                });
            }
        }
        private void processTable_Loaded(object sender, RoutedEventArgs e)
        {
            gridVisibility = true;
            ProcessGrid.ItemsSource = processTable.DefaultView;
            processTable.Columns.Add("opcode", typeof(string));
            processTable.Columns.Add("sn", typeof(string));
            processTable.Columns.Add("startTime", typeof(string));
            processTable.Columns.Add("endTime", typeof(string));
            processTable.Columns.Add("issue", typeof(string));
            List<HttpProductInformation> productInformations = HNHttp.GetProductInformation(0);
            productInformations.Sort((x1, x2) => x2._startTime.CompareTo(x1._startTime));
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            foreach (var item in productInformations)
            {
                DateTime _startTime = origin.AddMilliseconds(Double.Parse(item._startTime)).ToLocalTime();
                string startTime = _startTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");

                DateTime _endTime = origin.AddMilliseconds(Double.Parse(item._endTime)).ToLocalTime();
                string endTime = _endTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                DataRow receiveRow = processTable.NewRow();
                receiveRow["opcode"] = item._opcode;
                receiveRow["sn"] = item._serialNumber;
                receiveRow["startTime"] = startTime;
                receiveRow["endTime"] = endTime;
                receiveRow["issue"] = item._predict;
                processTable.Rows.Add(receiveRow);
            }
        }
        //socket을 통해 받은 product Info
        public void InputProductInformation(string ProductInfo, object data)
        {
            // drawMainChart스레드를 접근하기 위해서
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                InputCycleTimeAverage();
                InputRealTimeCount();
                SocketProductInformation productInformation = (SocketProductInformation)(data);
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                DateTime _startTime = origin.AddMilliseconds(Double.Parse(productInformation._startTime)).ToLocalTime();
                string startTime = _startTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");

                DateTime _endTime = origin.AddMilliseconds(Double.Parse(productInformation._endTime)).ToLocalTime();
                string endTime = _endTime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                DataRow receiveRow = processTable.NewRow();
                receiveRow["opcode"] = productInformation._opcode;
                receiveRow["sn"] = productInformation._serialNumber;
                receiveRow["startTime"] = startTime;
                receiveRow["endTime"] = endTime;
                receiveRow["issue"] = productInformation._predict;
                processTable.Rows.InsertAt(receiveRow, 0);
            }));
        }
        //porduct info 그리드 더블클릭 이벤트
        private void ProcessGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProcessGrid.SelectedItems.Count > 0)
            {
                DataRowView dataRowView = (DataRowView)ProcessGrid.SelectedItem;
                var opcode = dataRowView.Row.Field<string>("opcode");
                var sn = dataRowView.Row.Field<string>("sn");

                ProductCycleChart productCycleChart = new ProductCycleChart();
                productCycleChart.httpConn(opcode, sn);
                productCycleChart.ShowDialog();
            }
        }

        //FFT 이미지 확대 클릭 이벤트
        public void FFTImgExpansion(object sender, RoutedEventArgs e)
        {
            if (fftSource.Contains("no-image"))
            {
                string infoMsg = "Failed to load the image.";
                InfoAlert infoAlert = new InfoAlert(infoMsg);
                infoAlert.ShowDialog();
            }
            else
            {
                ImageExpansion imageExpansion = new ImageExpansion(fftSource);
                imageExpansion.ShowDialog();
            }
        }
        //FFT 현재 이미지 주소
        public void InputFFTImg(string src)
        {
            fftSource = src;
        }
        //history 차트 오픈 클릭 이벤트
        private void historyChartOpen(object sender, RoutedEventArgs e)
        {
            Graph_Back.SetValue(Grid.ColumnSpanProperty, 1);
            realTimeLoadSpindleSciChartSurface.SetValue(Grid.ColumnSpanProperty, 1);
            realTimeMaeSciChartSurface.SetValue(Grid.ColumnSpanProperty, 1);
            historyChartOpenBtn.Visibility = Visibility.Hidden;
            historyChartCloseBtn.Visibility = Visibility.Visible;
            Graph_Back.Margin = new Thickness(20, 5, 5, 5);
            realTimeLoadSpindleSciChartSurface.Margin = new Thickness(30, 10, 15, 10);
            realTimeMaeSciChartSurface.Margin = new Thickness(30, 10, 15, 23);

        }
        //history 차트 클로즈 클릭 이벤트
        private void historyChartClose(object sender, RoutedEventArgs e)
        {
            Graph_Back.SetValue(Grid.ColumnSpanProperty, 2);
            realTimeLoadSpindleSciChartSurface.SetValue(Grid.ColumnSpanProperty, 2);
            realTimeMaeSciChartSurface.SetValue(Grid.ColumnSpanProperty, 2);
            historyChartOpenBtn.Visibility = Visibility.Visible;
            historyChartCloseBtn.Visibility = Visibility.Hidden;
            Graph_Back.Margin = new Thickness(20, 5, 20, 5);
            realTimeLoadSpindleSciChartSurface.Margin = new Thickness(30, 10, 30, 10);
            realTimeMaeSciChartSurface.Margin = new Thickness(30, 10, 30, 23);
        }
        //제품 정보 dataGrid Sort
        private void ProductInfoListSortEvt(object sender, RoutedEventArgs e)
        {
            if (gridVisibility)
            {
                gridVisibility = false;
                App.Current.Resources["RowVisibility"] = Visibility.Collapsed;
            }
            else
            {
                gridVisibility = true;
                App.Current.Resources["RowVisibility"] = Visibility.Visible;
            }
        }
        //dataGrid Search
        private void ProductInfoListSearchEvt(object sender, RoutedEventArgs e)
        {
            SetProductInfoPeriod info = new SetProductInfoPeriod();
            info.ShowDialog();
        }

        //dataGrid Refresh
        private void ProductInfoListRefreshEvt(object sender, RoutedEventArgs e)
        {

        }
    }
} 