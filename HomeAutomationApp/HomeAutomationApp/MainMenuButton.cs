using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeAutomationApp
{
    public class MainMenuButton : ImageButton
    {
        
        public MainMenuButton()
        {
            try
            {
                this.Source = ImageSource.FromResource("HomeAutomationApp.Images.MainMenuButton.png", typeof(MainPage).Assembly);
                this.Aspect = Aspect.AspectFill;
                this.BackgroundColor = new Color(0, 0, 0, 0.01);
                this.WidthRequest = 80.0;
                this.HeightRequest = 60.0;
                this.Margin = new Thickness(35, 5, 5, 5);

                

                _tapGesture = new TapGestureRecognizer();
                _tapGesture.Tapped += TapGesture_Tapped;

                this.Clicked += MainMenuButton_Clicked;

            }
            catch
            {

            }
        }


        MainMenu _MainMenuControl;

        public MainMenu MainMenuControl { 
            get
            {
                return _MainMenuControl;
            }

            set
            {
                if (_MainMenuControl != null)
                {
                    _MainMenuControl.Navigated -= MainMenuControl_Navigated;
                }

                _MainMenuControl = value;

                if (_MainMenuControl != null)
                {
                    _MainMenuControl.Navigated += MainMenuControl_Navigated;
                }

            }
        }

        private async void MainMenuControl_Navigated(object source, EventArgs e)
        {
            this.FadeTo(1);
            if (_ExitControl != null)
                _ExitControl.IsVisible = false;
            if (MainMenuControl != null)
                await MainMenuControl.MakeDeactivationAnimation(MainLayout);
        }


        internal bool HandleBack()
        {
            if (_MainMenuControl.HandleBack())
            {
                this.FadeTo(1);
                if (_ExitControl != null)
                    _ExitControl.IsVisible = false;
                return true;
            }
            return false;
        }

        public Layout MainLayout { get; set; }
        TapGestureRecognizer _tapGesture;
        View _ExitControl = null;
        public View ExitControl
        {
            get
            {
                return _ExitControl;
            }
            set
            {

                if (_ExitControl != null)
                {
                    if (_ExitControl.GestureRecognizers.Contains(_tapGesture))
                        _ExitControl.GestureRecognizers.Remove(_tapGesture);
                }

                _ExitControl = value;

                if (_ExitControl != null)
                {
                    if (!_ExitControl.GestureRecognizers.Contains(_tapGesture))
                        _ExitControl.GestureRecognizers.Add(_tapGesture);
                }
            }

        }

        private async void TapGesture_Tapped(object sender, EventArgs e)
        {
            this.FadeTo(1);
            if (_ExitControl != null)
                _ExitControl.IsVisible = false;
            if (MainMenuControl != null)
                await MainMenuControl.MakeDeactivationAnimation(MainLayout);
        }

        private async void MainMenuButton_Clicked(object sender, EventArgs e)
        {
            this.FadeTo(0.05);
            if (_ExitControl != null)
                _ExitControl.IsVisible = true;
            if (MainMenuControl != null)
                await MainMenuControl.MakeActivationAnimation(MainLayout);
        }

    }
}
