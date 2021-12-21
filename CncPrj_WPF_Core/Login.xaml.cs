using HNInc.Communication.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : Page
    {
        public OpWindow opwindow;
        public Login login;
        public Login()
        {
            InitializeComponent();
            login = this;
            opwindow = new OpWindow(ref login);
        }
        private void LoginEvt(object sender, RoutedEventArgs e)
        {
            string id = idBox.Text;
            string pw = pwBox.Password;
            HttpAuthentication authentication = HNHttp.CheckAuthentication(id, pw);
            Debug.WriteLine(authentication._processResult);

            if (authentication._checkPassword)
            {
                // ID/PW 모두 일치하는 경우
                //MessageBox.Show("로그인 성공","알림", MessageBoxButton.OK, MessageBoxImage.Information);
                Uri uri = new Uri("/OpWindow.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
                opwindow.InputUserId(id);
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
