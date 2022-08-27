using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Button.ButtonContentLayout;

namespace HomeAutomationApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu : ContentView
    {
        public MainMenu()
        {
            InitializeComponent();


        }


        internal bool HandleBack()
        {
            if(_lastPage != null)
            {
                MakeDeactivationAnimation(_lastPage);
                _lastPage = null;
                return true;
            }
            return false;
        }

        ContentPage _parentPage = null;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (_parentPage == null)
            {
                var t = this.Parent;
                if (t == null)
                    return;
                while (t.Parent != null && !(t.Parent is NavigationPage))
                {
                    t = t.Parent;
                }

                if (t != null && t.Parent is NavigationPage)
                {
                    if (t is ContentPage)
                        _parentPage = t as ContentPage;
                    
                    if (t is MainPage)
                        btnMainPage.InputTransparent = true;
                    else if (t is TaskPage)
                        btnTask.InputTransparent = true;
                    else if (t is DownloadsPage)
                        btnMedias.InputTransparent = true;

                }
            }
        }

        Layout _lastPage = null;

        public async Task MakeActivationAnimation(Layout page)
        {
            if (page == null)
                return;

            _lastPage = page;
            page.ScaleTo(0.8, 250, Easing.CubicIn);
            await page.TranslateTo(200, 0, 250, Easing.CubicIn);
        }

        public async Task MakeDeactivationAnimation(Layout page)
        {
            if (page == null)
                return;

            if(_lastPage==page)
                _lastPage = null;
            page.ScaleTo(1, 250, Easing.CubicIn);
            await page.TranslateTo(0, 0, 250, Easing.CubicIn);
        }

        public EventHandler<EventArgs> Navigated;

        private async void btnChangePage_Clicked(object sender, EventArgs e)
        {
            var radio = sender as Button;
            if (radio == null)
                return;

            if (_parentPage == null)
                return;           
            
            if (radio == btnMainPage)
            {
                await this.Navigation.PushAsync(new MainPage());
            }
            else if (radio == btnTask)
            {
                await this.Navigation.PushAsync(new TaskPage());
            }
            else if (radio == btnMedias)
            {
                await this.Navigation.PushAsync(new DownloadsPage());
            }
            else if (radio == btnDomotique)
            {
                await this.Navigation.PushAsync(new HomeAutomationPage());
            }

            Navigated?.Invoke(this, EventArgs.Empty);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}