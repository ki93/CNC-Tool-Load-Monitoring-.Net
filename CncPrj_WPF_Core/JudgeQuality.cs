using CncPrj_WPF_Core.Alert;
using HNInc.Communication.Library;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfAnimatedGif;

namespace CncPrj_WPF_Core
{
    public class JudgeQuality
    {
        private OpWindow opwindow;
        HNHttp _hNHttp;
        //Image 설정 용
        string _currentPath;
        string _sn;
        string _result;
        Alerts _alerts;

        public JudgeQuality(ref OpWindow opwin)
        {
            opwindow = opwin;
            Debug.WriteLine(new Uri(ConfigurationManager.AppSettings.Get("WasUrl")));
            _hNHttp = new HNHttp(ConfigurationManager.AppSettings.Get("WasUrl"), ConfigurationManager.AppSettings.Get("AccountDBURL"), ConfigurationManager.AppSettings.Get("AccountDB"));
            _currentPath = AppDomain.CurrentDomain.BaseDirectory;
            _alerts = new Alerts();
        }

        public void InitJudgeQualityImage()
        {
            HttpQualityInformaiton httpQualityInformaiton = _hNHttp.GetQualityInformaiton();
            //최신 image 정보
            string productSerialNumber = null;
            string productPredictResult = null;
            string imageFilePath = null;
            string imagePath = Path.Combine(_currentPath, @"Image\");
            if (httpQualityInformaiton._requestResult.Equals("Success"))
            {
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }
                imageFilePath = Path.Combine(imagePath, httpQualityInformaiton._fileName);
                if (!File.Exists(imageFilePath))
                {
                    //image 저장 관련
                    using (MemoryStream memoryStream = new MemoryStream(httpQualityInformaiton._imageBytes))
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream))
                        {
                            imageFilePath = Path.Combine(imagePath, httpQualityInformaiton._fileName);
                            image.Save(imageFilePath);
                        };
                    };
                }
                productSerialNumber = httpQualityInformaiton._serialNumber;
                productPredictResult = $"{httpQualityInformaiton._predict}, {httpQualityInformaiton._accuracy}%";
            }
            else
            {
                imageFilePath = @"/Img/no-image.png";
                productSerialNumber = "No Data";
                productPredictResult = "No Data";
            }
            opwindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                opwindow.productSN.Content = productSerialNumber;
                opwindow.productResult.Content = productPredictResult;
                opwindow.productQualityImg.Source = new BitmapImage(new Uri(imageFilePath, UriKind.RelativeOrAbsolute));
                opwindow.productQualityImg.Stretch = Stretch.Fill;
                _sn = productSerialNumber;
                _result = productPredictResult;
                opwindow.InputFFTImg(imageFilePath, _sn, _result);

            }));
        }
        //품질 판정 start
        public void InputJudgeQualityStart(string eventName, object data)
        {
            SocketQualityInformation socketQualityInformation = (SocketQualityInformation)(data);
            string imagePath = @"/Img/loading2.gif";

            opwindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                opwindow.productSN.Content = socketQualityInformation._serialNumber;
                opwindow.productResult.Content = "Processing...";
                ImageBehavior.SetAnimatedSource(opwindow.productQualityImg, new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)));
                opwindow.productQualityImg.Stretch = Stretch.Uniform;
            }));
        }
        //품질 판정 end
        public void InputJudgeQualityEnd(string eventName, object data)
        {
            string currentMethod = MethodBase.GetCurrentMethod().Name;

            SocketQualityInformation quality = (SocketQualityInformation)data;
            bool checkImageSave = true;
            string imagePath = Path.Combine(_currentPath, @"Image\");
            string imageFilePath = null;
            try
            {
                imageFilePath = Path.Combine(imagePath, quality._fileName);
                if (!File.Exists(imageFilePath))
                {
                    //image 저장 관련
                    using (MemoryStream memoryStream = new MemoryStream(quality._imageBytes))
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream))
                        {
                            imageFilePath = Path.Combine(imagePath, quality._fileName);
                            image.Save(imageFilePath);
                        };
                    };
                }
                else
                {
                    //Debug.WriteLine($"Image File {quality._fileName} already exist");
                }
                checkImageSave = true;
            }
            catch (Exception e)
            {
                imageFilePath = @"/Img/no-image.png";
                checkImageSave = false;
                Task.Run(() =>
                {
                    opwindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _alerts.CreateAlert(AlertCategory.Warning, currentMethod, e.Message);
                    }));
                });

            }
            finally
            {
                opwindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    if (ImageBehavior.GetAnimationController(opwindow.productQualityImg) != null)
                    {
                        ImageBehavior.GetAnimationController(opwindow.productQualityImg).Dispose();
                    }
                    opwindow.productQualityImg.Source = new BitmapImage(new Uri(imageFilePath, UriKind.Absolute));
                    opwindow.productResult.Content = $"{quality._predict}, {quality._accuracy}%";
                    opwindow.productQualityImg.Stretch = Stretch.Fill;
                    opwindow.InputFFTImg(imageFilePath, _sn, _result);

                }));

                string imageInformation;
                if (checkImageSave)
                {
                    imageInformation = $"{quality._fileName},{quality._serialNumber},{quality._predict},{quality._accuracy},success";
                }
                else
                {
                    imageInformation = $"{quality._fileName},{quality._serialNumber},{quality._predict},{quality._accuracy},fail";
                }
            }
        }

    }
}