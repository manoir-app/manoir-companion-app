using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;

[assembly: ExportFont("Exo-Black.otf")]
[assembly: ExportFont("Exo-Bold.otf")]
[assembly: ExportFont("Exo-ExtraBold.otf")]
[assembly: ExportFont("Exo-ExtraLight.otf")]
[assembly: ExportFont("Exo-Italic.otf")]
[assembly: ExportFont("Exo-Light.otf")]
[assembly: ExportFont("Exo-Medium.otf")]
[assembly: ExportFont("Exo-Regular.otf")]
[assembly: ExportFont("Exo-SemiBold.otf")]
[assembly: ExportFont("Exo-Thin.otf")]
namespace HomeAutomationApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new CustomNavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }


        public void OnAppLinkReceived(string link)
        {
            if (this.MainPage != null)
            {
                var nav = this.MainPage as NavigationPage;
                var pg = new DeepLinkExecutingPage();
                nav.Navigation.PushModalAsync(pg);
            }
        }

        public Model.PlatformData PlatformData { get; set; }

        public void SetPlatformData(Model.PlatformData data)
        {
            this.PlatformData = data;
            if(this.MainPage!=null)
            {
                var nav = this.MainPage as NavigationPage;
                var pg = nav.CurrentPage;
                if(pg is MainPage)
                    (pg as MainPage).RefreshPlatformData();
            }
        }


        public event EventHandler<StartupTaskEventArgs> StartupTask;

        public void DoStartupTask(string code)
        {
            StartupTask?.Invoke(this, new StartupTaskEventArgs() { StartupTaskCode = code });
        }
    }

    public class StartupTaskEventArgs : EventArgs
    {
        public string StartupTaskCode { get; set; }
    }

}
