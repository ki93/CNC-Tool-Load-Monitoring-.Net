using HNInc.Communication.Library;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
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
        public OpWindow _opWindow;
        public Login()
        {
            InitializeComponent();

            _opWindow = new OpWindow();
            _opWindow._login = this;

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
            string id = idBox.Text.Trim();
            string pw = pwBox.Password.Trim();
            HttpAuthentication authentication = HNHttp.CheckAuthentication(id, pw);
            if (authentication._checkPassword)
            {
                // ID/PW 모두 일치하는 경우
                //MessageBox.Show("로그인 성공","알림", MessageBoxButton.OK, MessageBoxImage.Information);
                if (authentication._processResult.Equals("Success"))
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
                    NavigationService.LoadCompleted += _opWindow.NavigationServiceLoadCompleted;
                    NavigationService.Navigate(_opWindow, id);
                }
                else
                {
                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            ErrorAlert loginErrorAlert;
                            if (_opWindow._alerts.ContainsKey(authentication._processResult))
                            {
                                loginErrorAlert = (ErrorAlert)_opWindow._alerts[authentication._processResult];
                                loginErrorAlert.CountUp();
                            }
                            else
                            {
                                loginErrorAlert = new ErrorAlert(authentication._processResult, ref _opWindow);
                                _opWindow._alerts.Add(authentication._processResult, loginErrorAlert);
                                loginErrorAlert.ShowDialog();
                            }
                        }));
                    });
                }
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
            if (pwBox.Password.Length == 0)
            {
                pwBlock.Visibility = Visibility.Visible;
            }
            else
            {
                pwBlock.Visibility = Visibility.Hidden;
            }
        }
    }
}
