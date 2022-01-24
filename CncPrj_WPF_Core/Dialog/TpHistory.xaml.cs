using CncPrj_WPF_Core.Alert;
using HNInc.Communication.Library;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// TpHistory.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TpHistory : Window
    {
        HNHttp _hNHttp;
        private DataTable dailyTable;
        private DataTable weeklyTable;
        private DataTable monthlyTable;
        private DataTable searchTable;
        public TpHistory tpHistory;
        public DateTime searchStartTIme;
        public DateTime searchEndTime;
        public string searchTableDate;
        Alerts _alerts;

        public TpHistory()
        {
            InitializeComponent();
            _alerts = new Alerts();

            string currentMethod = MethodBase.GetCurrentMethod().Name;
            _hNHttp = new HNHttp(ConfigurationManager.AppSettings.Get("WasUrl"), ConfigurationManager.AppSettings.Get("AccountDBURL"), ConfigurationManager.AppSettings.Get("AccountDB"));

            tpHistory = this;
            dailyTable = new DataTable();
            weeklyTable = new DataTable();
            monthlyTable = new DataTable();
            searchTable = new DataTable();
            searchTable.Columns.Add("Date", typeof(string));
            searchTable.Columns.Add("Count", typeof(string));
            dailyTable.Columns.Add("Date", typeof(string));
            dailyTable.Columns.Add("Count", typeof(string));
            weeklyTable.Columns.Add("Date", typeof(string));
            weeklyTable.Columns.Add("Count", typeof(string));
            monthlyTable.Columns.Add("Date", typeof(string));
            monthlyTable.Columns.Add("Count", typeof(string));
            try
            {
                List<HttpProductCounts> dayProductCounts = _hNHttp.GetProductCountsList(DateTime.Now.AddDays(-100), DateTime.Now, HttpOPCode.OP10_3, HttpClassification.day);
                DayInputProductCounts(dayProductCounts);
                List<HttpProductCounts> weekProductCounts = _hNHttp.GetProductCountsList(DateTime.Now.AddMonths(-5), DateTime.Now, HttpOPCode.OP10_3, HttpClassification.week);
                WeekInputProductCounts(weekProductCounts);
                List<HttpProductCounts> monthProductCounts = _hNHttp.GetProductCountsList(DateTime.Now.AddMonths(-5), DateTime.Now, HttpOPCode.OP10_3, HttpClassification.month);
                MonthInputProductCounts(monthProductCounts);
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Error, currentMethod, ex.ToString());
                    }));
                });

            }
        }

        //search Start Date 입력
        public void startDatePick_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //endDatePick.DisplayDateStart = startDatePick.SelectedDate;
            var startTIme = startDatePick.SelectedDate.ToString();
            searchStartTIme = DateTime.Parse(startTIme).ToLocalTime();
        }
        //search End Date 입력
        public void endDatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var endTime = endDatePick.SelectedDate.ToString();
            searchEndTime = DateTime.Parse(endTime).ToLocalTime();
        }
        //window close
        private void CloseProductWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //일별 데이터 그리드 추가
        public void DayInputProductCounts(List<HttpProductCounts> dayProductCounts)
        {
            dayProductCounts.Reverse();
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            try
            {
                foreach (HttpProductCounts item in dayProductCounts)
                {
                    DateTime dateTime = DateTime.Parse(item._date).ToLocalTime();
                    string date = dateTime.ToString("yyyy'-'MM'-'dd");
                    string count = item._count.ToString();
                    dailyTable.Rows.Add(date, count);
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Error, currentMethod, ex.ToString());
                    }));
                });
            }
        }
        //주별 데이터 그리드 추가
        public void WeekInputProductCounts(List<HttpProductCounts> weekProductCounts)
        {
            weekProductCounts.Reverse();
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            try
            {
                foreach (HttpProductCounts item in weekProductCounts)
                {
                    DateTime dateTime = DateTime.Parse(item._date).ToLocalTime();
                    string date = dateTime.ToString("yyyy'-'MM'-'dd");
                    string count = item._count.ToString();
                    weeklyTable.Rows.Add(date, count);
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Error, currentMethod, ex.ToString());
                    }));
                });
            }
        }
        //월별 데이터 그리드 추가
        public void MonthInputProductCounts(List<HttpProductCounts> monthProductCounts)
        {
            monthProductCounts.Reverse();
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            try
            {
                foreach (HttpProductCounts item in monthProductCounts)
                {
                    DateTime dateTime = DateTime.Parse(item._date).ToLocalTime();
                    string date = dateTime.ToString("yyyy'-'MM'-'dd");
                    string count = item._count.ToString();
                    monthlyTable.Rows.Add(date, count);
                }
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Error, currentMethod, ex.ToString());
                    }));
                });
            }
        }
        //서치 데이터 그리드 추가
        public void SearchInputProductCounts(List<HttpProductCounts> searchProductCounts)
        {
            int searchTableCount = 0;
            searchProductCounts.Reverse();
            foreach (HttpProductCounts item in searchProductCounts)
            {
                searchTableCount += item._count;
            }
            searchTableDate = searchStartTIme.ToString().Substring(0, 10) + " ~ " + searchEndTime.ToString().Substring(0, 10);
            searchTable.Rows.Add(searchTableDate, searchTableCount);
        }

        //그리드 로드
        private void dailyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            dailyGrid.ItemsSource = dailyTable.DefaultView;
 
        }

        private void weeklyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            weeklyGrid.ItemsSource = weeklyTable.DefaultView;
        }

        private void monthlyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            monthlyGrid.ItemsSource = monthlyTable.DefaultView;
        }

        private void searchGrid_Loaded(object sender, RoutedEventArgs e)
        {
            searchGrid.ItemsSource = searchTable.DefaultView;
        }
        //

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

            switch (tabItem)
            {
                case "DAILY":
                    //dailyTable.Rows.Clear();
                    //dailyTable.Columns.Clear();
                    //List<HttpProductCounts> dayProductCounts = HNHttp.GetProductCountsRequest(DateTime.Now.AddDays(-100), DateTime.Now, HttpOPCode.OP10_3, HttpClassification.day);
                    //DayInputProductCounts(dayProductCounts);
                    break;

                case "WEEKLY":
                    //weeklyTable.Rows.Clear();
                    //weeklyTable.Columns.Clear();
                    //List<HttpProductCounts> weekProductCounts = HNHttp.GetProductCountsRequest(DateTime.Now.AddMonths(-5), DateTime.Now, HttpOPCode.OP10_3, HttpClassification.week);
                    //WeekInputProductCounts(weekProductCounts);
                    break;

                case "MONTHLY":
                    //monthlyTable.Rows.Clear();
                    //monthlyTable.Columns.Clear();
                    //List<HttpProductCounts> monthProductCounts = HNHttp.GetProductCountsRequest(DateTime.Now.AddMonths(-5), DateTime.Now, HttpOPCode.OP10_3, HttpClassification.month);
                    //MonthInputProductCounts(monthProductCounts);
                    break;

                case "SEARCH":
                    //searchTable.Rows.Clear();
                    startDatePick.DisplayDateEnd = DateTime.Today;
                    endDatePick.DisplayDateEnd = DateTime.Today;
                    startDatePick.SelectedDate = DateTime.Today;
                    endDatePick.SelectedDate = DateTime.Today;
                    break;

                default:
                    return;
            }
        }
        //서치 버튼 이벤트
        private void phSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            List<HttpProductCounts> searchProductCounts = _hNHttp.GetProductCountsList(searchStartTIme, searchEndTime.AddDays(1), HttpOPCode.OP10_3, HttpClassification.day);
            SearchInputProductCounts(searchProductCounts);
        }
    }
}