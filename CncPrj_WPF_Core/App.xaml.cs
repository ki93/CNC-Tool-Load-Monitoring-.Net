using System.Configuration;
using System.Windows;
using SciChart.Charting.Visuals;

namespace CncPrj_WPF_Core
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Set this code once in App.xaml.cs or application startup before any SciChartSurface is shown 
            // Set this code once in App.xaml.cs or application startup
            SciChartSurface.SetRuntimeLicenseKey(ConfigurationManager.AppSettings.Get("Scichart"));
        }
    }
}
