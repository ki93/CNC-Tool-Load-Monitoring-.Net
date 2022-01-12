using HNInc.Communication.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Navigation;
using CncPrj_WPF_Core.Alert;
using System.Reflection;

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
        string judgeSn;
        string judgeResult;
        HNSocketIO _hNSocketIO;
        public OpWindow opwindow;
        JudgeQuality _judgeQuality;
        DispatcherTimer _timer;
        DispatcherTimer _deviceHealthChecktimer;
        BrushConverter converter = new BrushConverter();
        bool maeFlag = true;
        private DataTable processTable;
        bool _gridVisibility;
        DateTime _productInfoPeriodStarttime;
        DateTime _productInfoPeriodEndtime;
        Alerts _alerts;

        public OpWindow()
        {
            InitializeComponent();
        }

        public void NavigationServiceLoadCompleted(object sender, NavigationEventArgs e)
        {
            opwindow = this;
            drawMainChart = new DrawMainChart(ref opwindow);
            _judgeQuality = new JudgeQuality(ref opwindow);
            processTable = new DataTable();
            _alerts = new Alerts();

            _timer = new DispatcherTimer(); //호출 함수 설정
            _timer.Tick += timer_Tick; //함수 호출 주기 설정
            _timer.Interval = TimeSpan.FromSeconds(0); //타이머 시작
            _timer.Start();

            _deviceHealthChecktimer = new DispatcherTimer();
            _deviceHealthChecktimer.Tick += InputDeiviceHealthCheck;
            _deviceHealthChecktimer.Interval = TimeSpan.FromSeconds(60);
            _deviceHealthChecktimer.Start();

            if (_hNSocketIO == null)
            {
                _hNSocketIO = new HNSocketIO(new Uri("http://9.8.100.153:8082/"), SocketIOClient.Transport.TransportProtocol.WebSocket, new TimeSpan(TimeSpan.TicksPerMinute), 5);
                _hNSocketIO.Connect();

                _hNSocketIO.SocketConnectedEvent += SocketOnConnected;

                _hNSocketIO.ReceiveRealTimeLoadData();
                _hNSocketIO.RealTimeLoadEvent += InputRealTimeLoadData;

                _hNSocketIO.ReceiveRealTimeLossData();
                _hNSocketIO.RealTimeLossEvent += InputRealTimeLossData;

                _hNSocketIO.ReceiveQualityJudgmentProgresStart();
                _hNSocketIO.QualityJudgmentProgresStartEvent += _judgeQuality.InputJudgeQualityStart;

                _hNSocketIO.ReceiveQualityJudgmentProgressEnd();
                _hNSocketIO.QualityJudgmentProgresEndEvent += _judgeQuality.InputJudgeQualityEnd;

                _hNSocketIO.ReceiveProductInformation();
                _hNSocketIO.ProductInformationEvent += InputProductInformation;
            }

            ComboBoxItem historyChartGroupByItem = (ComboBoxItem)historyChartGroupByValue.SelectedItem;
            hitoryGroupByTime = (string)historyChartGroupByItem.Content;
            hitoryStartTime = DateTime.Now.AddMinutes(-15).ToString();
            hitoryEndTime = DateTime.Now.ToString();
            historyChartInit();
            InputCycleTimeAverage();
            InputRealTimeCount();
            _judgeQuality.InitJudgeQualityImage();
            _productInfoPeriodStarttime = DateTime.Today;
            _productInfoPeriodEndtime = DateTime.Today;

            userId.Text = e.ExtraData.ToString();
            NavigationService.LoadCompleted -= NavigationServiceLoadCompleted;
        }
        void SocketOnConnected(Object a, Object b)
        {
            _hNSocketIO.SendData(SocketEventNames.RTLoadData);
            _hNSocketIO.SendData(SocketEventNames.RTLossData);
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
            string currentMethod = MethodBase.GetCurrentMethod().Name;
            HttpRealTimeCount httpRealTimeCount = HNHttp.GetRealTimeCount(HttpOPCode.OP10_3);
            if (httpRealTimeCount._requestResult.Equals("Success"))
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    totalproductVal.Content = httpRealTimeCount._count;
                }));
            }
            else
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Error, currentMethod, httpRealTimeCount._requestResult);
                    }));
                });
            }
        }
        //cycle time 5개 평균 데이터 출력
        public void InputCycleTimeAverage()
        {
            HttpCycleTimeAverage httpCycleTimeAverage = HNHttp.GetCycleTimeAverage(5);
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            if (httpCycleTimeAverage._requestResult.Equals("Success"))
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(httpCycleTimeAverage._mean);
                    cycleTimeVal.Content = $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
                }));
            }
            else
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Error, currentMethod, httpCycleTimeAverage._requestResult);
                    }));
                });
            }
        }
        //가동 상태 데이터 출력
        public void InputDeiviceHealthCheck(object sender, EventArgs e)
        {
            HttpDeiviceHealthCheck httpDeiviceHealthCheck = HNHttp.GetDeiviceHealthCheck(HttpOPCode.OP10_3);
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            if (httpDeiviceHealthCheck._requestResult.Equals("Success"))
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    if (httpDeiviceHealthCheck._status == "success")
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
            else
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Error, currentMethod, httpDeiviceHealthCheck._requestResult);
                    }));
                });
            }
        }

        //상단 시간 출력 및 가동 상태 체크
        public void timer_Tick(object sender, EventArgs e)
        {
            currentTime.Text = System.DateTime.Now.ToString("yyyy-MM-dd ddd HH:mm:ss");
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
            _hNSocketIO.SocketConnectedEvent -= SocketOnConnected;
            _hNSocketIO.RealTimeLoadEvent -= InputRealTimeLoadData;
            _hNSocketIO.RealTimeLossEvent -= InputRealTimeLossData;
            _hNSocketIO.QualityJudgmentProgresStartEvent -= _judgeQuality.InputJudgeQualityStart;
            _hNSocketIO.QualityJudgmentProgresEndEvent -= _judgeQuality.InputJudgeQualityEnd;
            _hNSocketIO.ProductInformationEvent -= InputProductInformation;
            _hNSocketIO.Disconnect();
            _hNSocketIO = null;

            drawMainChart = null;
            opwindow = null;
            _judgeQuality = null;
            processTable = null;
            _alerts = null;
            _timer.Tick -= timer_Tick;
            _timer.Stop();
            _timer = null;
            _deviceHealthChecktimer.Tick -= InputDeiviceHealthCheck;
            _deviceHealthChecktimer.Stop();
            _deviceHealthChecktimer = null;
            Login login = new Login();
            NavigationService.LoadCompleted += login.NavigationServiceLoadCompleted;
            NavigationService.Navigate(login);
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
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            //data http로 보내기
            if (hitoryStartTime != null)
            {
                historyChartLoadMsg.Visibility = Visibility.Visible;

                //List<SpindleLoad> spindleLoads = HNHttp.GetSpindleLoadRequest((int)Convert.ToDateTime(hitoryStartTime).Year, (int)Convert.ToDateTime(hitoryStartTime).Month, (int)Convert.ToDateTime(hitoryStartTime).Day, (int)Convert.ToDateTime(hitoryEndTime).Year, (int)Convert.ToDateTime(hitoryEndTime).Month, (int)Convert.ToDateTime(hitoryEndTime).Day, OPCode.OP10_3, hitoryGroupByTime);
                Task.Run(() =>
                {
                    List<HttpSpindleLoad> spindleLoads = HNHttp.GetSpindleLoadList(Convert.ToDateTime(hitoryStartTime).ToUniversalTime(), Convert.ToDateTime(hitoryEndTime).ToUniversalTime(), HttpOPCode.OP10_3, hitoryGroupByTime);

                    if (spindleLoads.Count==0)
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            historyChartLoadMsg.Visibility = Visibility.Hidden;
                        }));
                        Task.Run(() =>
                        {
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                            {
                                _alerts.CreateAlert(AlertCategory.Information, currentMethod, "No Data");
                            }));
                        });
                    }
                    else if (spindleLoads[spindleLoads.Count - 1]._requestResult.Equals("Success"))
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            drawMainChart.HistoryChart(spindleLoads);
                            historyChartLoadMsg.Visibility = Visibility.Hidden;
                        }));
                    }
                    else
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            historyChartLoadMsg.Visibility = Visibility.Hidden;
                        }));
                        Task.Run(() =>
                        {
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                            {
                                _alerts.CreateAlert(AlertCategory.Error, currentMethod, spindleLoads[spindleLoads.Count - 1]._requestResult);
                            }));
                        });
                    }
                });
            }
        }
        private void processTable_Loaded(object sender, RoutedEventArgs e)
        {
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            _gridVisibility = true;
            ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/Select.png", UriKind.RelativeOrAbsolute));
            ProductInfoListCalendarBtnImage.Source = new BitmapImage(new Uri(@"/Img/calendar.png", UriKind.RelativeOrAbsolute));
            if (processTable.Columns.Count == 0)
            {
                ProcessGrid.ItemsSource = processTable.DefaultView;
                processTable.Columns.Add("opcode", typeof(string));
                processTable.Columns.Add("sn", typeof(string));
                processTable.Columns.Add("startTime", typeof(string));
                processTable.Columns.Add("endTime", typeof(string));
                processTable.Columns.Add("issue", typeof(string));
            }
            else
            {
                processTable.Clear();
            }
            List<HttpProductInformation> productInformations = HNHttp.GetProductInformationList(0);
            if (productInformations.Count==0)
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Information, currentMethod, "No Data Today");
                    }));
                });
            }
            else if (productInformations[productInformations.Count - 1]._requestResult.Equals("Success"))
            {
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
            else
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Error, currentMethod, productInformations[productInformations.Count - 1]._requestResult);
                    }));
                });
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
        //제품 정보 dataGrid Sort
        private void ProductInfoListSortEvt(object sender, RoutedEventArgs e)
        {
            if (_gridVisibility)
            {
                _gridVisibility = false;
                if (ProductInfoListSortBtnImage.IsMouseOver)
                {
                    ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/filter_pressed_mouseover.png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/filter_pressed.png", UriKind.RelativeOrAbsolute));
                }
                App.Current.Resources["RowVisibility"] = Visibility.Collapsed;
            }
            else
            {
                _gridVisibility = true;
                if (ProductInfoListSortBtnImage.IsMouseOver)
                {
                    ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/Select_Up.png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/Select.png", UriKind.RelativeOrAbsolute));
                }
                App.Current.Resources["RowVisibility"] = Visibility.Visible;

            }
        }
        ////제품 정보 dataGrid MouseOver
        private void ProductInfoSortBtmMouseEnter(object sender, MouseEventArgs e)
        {

            if (_gridVisibility)
            {
                ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/Select_Up.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/filter_pressed_mouseover.png", UriKind.RelativeOrAbsolute));
            }
        }
        private void ProductInfoSortBtmMouseLeave(object sender, MouseEventArgs e)
        {

            if (_gridVisibility)
            {
                ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/Select.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/filter_pressed.png", UriKind.RelativeOrAbsolute));
            }
        }
        //dataGrid Search
        private void ProductInfoListSearchEvt(object sender, RoutedEventArgs e)
        {
            SetProductInfoPeriod info = new SetProductInfoPeriod(ref opwindow, _productInfoPeriodStarttime, _productInfoPeriodEndtime);
            info.ShowDialog();
        }
        //dataGrid Search Request
        public void RequestProductInfoList(DateTime starttime, DateTime endtime)
        {
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            _productInfoPeriodStarttime = starttime;
            _productInfoPeriodEndtime = endtime;
            if (starttime.Equals(DateTime.Today))
            {
                ProductInfoListCalendarBtnImage.Source = new BitmapImage(new Uri(@"/Img/calendar.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                ProductInfoListCalendarBtnImage.Source = new BitmapImage(new Uri(@"/Img/calendar_pressed.png", UriKind.RelativeOrAbsolute));
            }
            ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/Select.png", UriKind.RelativeOrAbsolute));
            TimeSpan timespanFromToday = DateTime.Today.Subtract(starttime);
            int daysFromToday = timespanFromToday.Days;
            _gridVisibility = true;
            App.Current.Resources["RowVisibility"] = Visibility.Visible;
            processTable.Clear();
            List<HttpProductInformation> productInformations = HNHttp.GetProductInformationList(daysFromToday);
            if (productInformations.Count==0)
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Information, currentMethod, "No Data");
                    }));
                });
            }
            else if (productInformations[productInformations.Count - 1]._requestResult.Equals("Success"))
            {
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
            else
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Error, currentMethod, productInformations[productInformations.Count - 1]._requestResult);
                    }));
                });
            }
        }
        private void ProductInfoCalendarBtmMouseEnter(object sender, MouseEventArgs e)
        {
            if (_productInfoPeriodStarttime.Equals(DateTime.Today))
            {
                ProductInfoListCalendarBtnImage.Source = new BitmapImage(new Uri(@"/Img/calendar_Up.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                ProductInfoListCalendarBtnImage.Source = new BitmapImage(new Uri(@"/Img/calendar_pressed_mouseover.png", UriKind.RelativeOrAbsolute));
            }
        }

        private void ProductInfoCalendarBtmMouseLeave(object sender, MouseEventArgs e)
        {
            if (_productInfoPeriodStarttime.Equals(DateTime.Today))
            {
                ProductInfoListCalendarBtnImage.Source = new BitmapImage(new Uri(@"/Img/calendar.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                ProductInfoListCalendarBtnImage.Source = new BitmapImage(new Uri(@"/Img/calendar_pressed.png", UriKind.RelativeOrAbsolute));
            }
        }

        //dataGrid Refresh
        private void ProductInfoListRefreshEvt(object sender, RoutedEventArgs e)
        {
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            _gridVisibility = true;
            ProductInfoListSortBtnImage.Source = new BitmapImage(new Uri(@"/Img/Select.png", UriKind.RelativeOrAbsolute));
            ProductInfoListCalendarBtnImage.Source = new BitmapImage(new Uri(@"/Img/calendar.png", UriKind.RelativeOrAbsolute));
            _productInfoPeriodStarttime = DateTime.Today;
            _productInfoPeriodEndtime = DateTime.Today;
            App.Current.Resources["RowVisibility"] = Visibility.Visible;
            processTable.Clear();
            List<HttpProductInformation> productInformations = HNHttp.GetProductInformationList(0);
            if (productInformations.Count==0)
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Information, currentMethod, "No Data");
                    }));
                });
            }
            else if (productInformations[productInformations.Count - 1]._requestResult.Equals("Success"))
            {
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
            else
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Error, currentMethod, productInformations[productInformations.Count - 1]._requestResult);
                    }));
                });
            }
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
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            if (fftSource.Contains("no-image"))
            {
                string infoMsg = "Failed to load the image.";
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Information, currentMethod, infoMsg);
                    }));
                });
            }
            else
            {
                ImageExpansion imageExpansion = new ImageExpansion(fftSource, judgeSn, judgeResult);
                imageExpansion.ShowDialog();
            }
        }
        //FFT 현재 이미지 주소
        public void InputFFTImg(string src, string sn, string result)
        {
            fftSource = src;
            judgeSn = sn;
            judgeResult = result;
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
    }
}
