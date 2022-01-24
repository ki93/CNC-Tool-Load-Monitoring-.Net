using CncPrj_WPF_Core.Alert;
using HNInc.Communication.Library;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : Page
    {
        IsolatedStorageFile _isoStore;
        Alerts _alerts;
        HNHttp _hNHttp;

        public Login()
        {
            InitializeComponent();

            _alerts = new Alerts();
            _hNHttp = new HNHttp(ConfigurationManager.AppSettings.Get("WasUrl"), ConfigurationManager.AppSettings.Get("AccountDBURL"), ConfigurationManager.AppSettings.Get("AccountDB"));

            _isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (_isoStore.FileExists("IDRememberMe.txt"))
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("IDRememberMe.txt", FileMode.Open, _isoStore))
                {
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        idBox.Text = reader.ReadToEnd();
                    }
                }
            }
        }
        public void OnLoad(object sender, EventArgs e)
        {
            if (_isoStore.FileExists("IDRememberMe.txt"))
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("IDRememberMe.txt", FileMode.Open, _isoStore))
                {
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        idBox.Text = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                idBox.Text = "";
            }
        }
        public void NavigationServiceLoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_isoStore.FileExists("IDRememberMe.txt"))
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("IDRememberMe.txt", FileMode.Open, _isoStore))
                {
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        idBox.Text = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                idBox.Text = "";
            }
            NavigationService.LoadCompleted -= NavigationServiceLoadCompleted;
        }
        private void LoginEvt(object sender, RoutedEventArgs e)
        {
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            string id = idBox.Text.Trim();
            string pw = pwBox.Password.Trim();

            HttpAuthentication authentication = _hNHttp.CheckAuthentication(id, pw);
            
            if (authentication._checkPassword)
            {
                if ((bool)uRememberMe.IsChecked)
                {
                    if (!_isoStore.FileExists("IDRememberMe.txt"))
                    {
                        using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("IDRememberMe.txt", FileMode.CreateNew, _isoStore))
                        {
                            using (StreamWriter writer = new StreamWriter(isoStream))
                            {
                                writer.Write(id);
                            }
                        }
                    }
                }
                else
                {
                    if (_isoStore.FileExists("IDRememberMe.txt"))
                    {
                        _isoStore.DeleteFile("IDRememberMe.txt");
                    }
                }
                MoveStep _moveStep = new MoveStep();
                NavigationService.LoadCompleted += _moveStep.NavigationServiceLoadCompleted;
                NavigationService.Navigate(_moveStep, id);
            }
            else if (authentication._processResult.Contains("Fail"))
            {
                _alerts.CreateAlert(AlertCategory.Error, currentMethod, authentication._processResult);
            }
            else
            {
                if (id.Trim().Equals("") || pw.Trim().Equals(""))
                {
                    if (id.Trim().Equals(""))
                    {
                        idBlock.Text = "ID를 입력해주세요";
                        idBox.Focus();
                        if (pw.Trim().Equals(""))
                        {
                            pwBlock.Text = "비밀번호를 입력해주세요";
                        }
                    }
                    else
                    {
                        pwBlock.Text = "비밀번호를 입력해주세요";
                        pwBox.Focus();
                    }
                }
                else
                {
                    // 둘 다 일치하지 않는 경우
                    idBox.Focus();
                    logResult.Text = "아이디와 비밀번호가 일치하지 않습니다.";
                }
            }
        }

        private void LoginEvtKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LoginEvt(sender, e);
            }
        }

        private void PwBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            logResult.Text = "";
            if (pwBox.Password.Length == 0)
            {
                pwBlock.Visibility = Visibility.Visible;
            }
            else
            {
                pwBlock.Visibility = Visibility.Hidden;
            }
        }

        private void idBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            logResult.Text = "";
        }
    }
}
