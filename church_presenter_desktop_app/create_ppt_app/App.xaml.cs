using create_ppt_app.Model;
using create_ppt_app.utils;
using create_ppt_app.ViewModel;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace create_ppt_app
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // show custom dynamic splash screen here

            await ApplicationSettingsViewModel.Instance.Initialize();

            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();
            MainWindow = new MainWindow(mainWindowViewModel);
            MainWindow.DataContext = mainWindowViewModel;
            MainWindow.Show();
        }
    }

}
