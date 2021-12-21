using HNInc.Communication.Library;
using System;
using System.Diagnostics;
using System.IO;
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
        //Image 설정 용
        string _currentPath;

        public JudgeQuality(ref OpWindow opwin)
        {
            opwindow = opwin;
            _currentPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void InitJudgeQualityImage()
        {
            string recentImageInformationPath = Path.Combine(_currentPath, @"RecentImageName\");
            string fileName = "RecentImageName.txt";
            string recentImageInformationFilePath = Path.Combine(recentImageInformationPath, fileName);
            //최신 image 정보
            string productSerialNumber = null;
            string productPredictResult = null;
            string imagePath = Path.Combine(_currentPath, @"Image\");
            string imageFilePath = null;

            if (!Directory.Exists(recentImageInformationPath))
            {
                Directory.CreateDirectory(recentImageInformationPath);
            }
            if (!File.Exists(recentImageInformationFilePath))
            {
                FileStream fileStream = File.Create(recentImageInformationFilePath);
                fileStream.Close();
                imageFilePath = Path.Combine(imagePath, "no-image.png");
                productSerialNumber = "No Data";
                productPredictResult = "No Data";
            }
            string imageInformation = File.ReadAllText(recentImageInformationFilePath);
            if (imageInformation.Length != 0)
            {
                string[] imageInformations = imageInformation.Split(",");
                productSerialNumber = imageInformations[1];
                productPredictResult = $"{imageInformations[2]}, {imageInformations[3]}%";
                imageFilePath = Path.Combine(_currentPath, imageInformations[0]);
                if (imageInformations[4].Equals("success"))
                {
                    imageFilePath = Path.Combine(imagePath, imageInformations[0]);
                }
                else
                {
                    imageFilePath = Path.Combine(imagePath, "no-image.png");
                }
            }
            else
            {
                imageFilePath = Path.Combine(imagePath, "no-image.png");
                productSerialNumber = "No Data";
                productPredictResult = "No Data";
            }
            opwindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                opwindow.productSN.Content = productSerialNumber;
                opwindow.productResult.Content = productPredictResult;
                opwindow.productQualityImg.Source = new BitmapImage(new Uri(imageFilePath, UriKind.Absolute));
                opwindow.productQualityImg.Stretch = Stretch.Fill;
                opwindow.InputFFTImg(imageFilePath);

            }));
        }
        //품질 판정 start
        public void InputJudgeQualityStart(string eventName, object data)
        {
            SocketQualityInformation quality = (SocketQualityInformation)(data);
            string imagePath = Path.Combine(_currentPath, @"Image\loading2.gif");

            opwindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                opwindow.productSN.Content = quality._serialNumber;
                opwindow.productResult.Content = "Processing...";
                ImageBehavior.SetAnimatedSource(opwindow.productQualityImg, new BitmapImage(new Uri(imagePath, UriKind.Absolute)));
                opwindow.productQualityImg.Stretch = Stretch.Uniform;
            }));
        }
        //품질 판정 end
        public void InputJudgeQualityEnd(string eventName, object data)
        {
            SocketQualityInformation quality = (SocketQualityInformation)data;
            bool checkImageSave = true;
            string imagePath = Path.Combine(_currentPath, @"Image\");
            //과거 최신 image 정보 관련
            string recentImageInformationPath = Path.Combine(_currentPath, @"RecentImageName\");
            string fileName = "RecentImageName.txt";
            string recentImageInformationFilePath = Path.Combine(recentImageInformationPath, fileName);
            //image 저장 실패 alert
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
                    Debug.WriteLine($"Image File {quality._fileName} already exist");
                }
                checkImageSave = true;
            }
            catch (Exception e)
            {
                //  Show Dialog로 교체 예정
                //messageBoxResult = MessageBox.Show(e.Message, "HN Inc", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(e.ToString());
                imageFilePath = Path.Combine(imagePath, "no-image.png");
                checkImageSave = false;
                Task.Run(() =>
                {
                    opwindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        WarningAlert warningAlert = new WarningAlert(e.Message);
                        warningAlert.ShowDialog();
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
                    opwindow.InputFFTImg(imageFilePath);
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
                File.WriteAllText(recentImageInformationFilePath, imageInformation);
            }
        }

    }
}