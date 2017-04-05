using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OrderCalculator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //base.OnStartup(e);
            MainWindow window = new OrderCalculator.MainWindow();

            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null)
            {
                string commandLineFile =
                    AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0];

                Uri fileUri = new Uri(commandLineFile);
                string filePath = Uri.UnescapeDataString(fileUri.AbsolutePath);

                if (File.Exists(filePath))
                    Application.Current.Resources["AppData"] = new MainViewModel(filePath);
            }

            window.Show();
        }
    }
}
