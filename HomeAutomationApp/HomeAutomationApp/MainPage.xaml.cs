using HomeAutomationApp.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;

namespace HomeAutomationApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            CurrentTab = 0;
            InitializeComponent();
            ShowLoading = true;
            this.BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);

            imgFond.Source = ImageSource.FromResource("HomeAutomationApp.Images.fond-house.png", typeof(MainPage).Assembly);
            imgHome.Source = ImageSource.FromResource("HomeAutomationApp.Images.streamline-icon-single-man-circle.png", typeof(MainPage).Assembly);
            imgHouse.Source = ImageSource.FromResource("HomeAutomationApp.Images.streamline-icon-real-estate-search-house-1.png", typeof(MainPage).Assembly);
            imgChat.Source = ImageSource.FromResource("HomeAutomationApp.Images.streamline-icon-messages-bubble-typing-1.png", typeof(MainPage).Assembly);
            imgTodo.Source = ImageSource.FromResource("HomeAutomationApp.Images.streamline-icon-style-three-pin-check.png", typeof(MainPage).Assembly);

            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += btnHome_Tapped;
            btnHome.GestureRecognizers.Add(tapGesture);
            tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += btnHouse_Tapped;
            btnHouse.GestureRecognizers.Add(tapGesture);
            tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += btnChat_Tapped;
            btnChat.GestureRecognizers.Add(tapGesture);
            tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += btnTodo_Tapped;
            btnTodo.GestureRecognizers.Add(tapGesture);

            MessagingCenter.Subscribe<string, IDictionary<string, string>>(this, "greetings_change", (sender, a) =>
            {

            });

            RefreshTabs();
        }

        protected override bool OnBackButtonPressed()
        {
            if (btnMainMenu.HandleBack())
                return true;

            return base.OnBackButtonPressed();
        }

        Color _inactiveTab = (Color)new ColorTypeConverter().ConvertFromInvariantString("#36426C");
        Color _activeTab = (Color)new ColorTypeConverter().ConvertFromInvariantString("#232B46");

        private void btnHome_Tapped(object sender, EventArgs e)
        {
            CurrentTab = 0;
            OnPropertyChanged("CurrentTab");

            RefreshTabs();
        }

        private void btnHouse_Tapped(object sender, EventArgs e)
        {
            CurrentTab = 1;
            OnPropertyChanged("CurrentTab");

            RefreshTabs();
        }
        private void btnChat_Tapped(object sender, EventArgs e)
        {
            CurrentTab = 2;
            OnPropertyChanged("CurrentTab");

            RefreshTabs();
        }
        private void btnTodo_Tapped(object sender, EventArgs e)
        {
            CurrentTab = 3;
            OnPropertyChanged("CurrentTab");

            RefreshTabs();
        }

        private void RefreshTabs()
        {
            switch (CurrentTab)
            {
                case 0:
                    VisualStateManager.GoToState(this, "Tab1");
                    break;
                case 1:
                    VisualStateManager.GoToState(this, "Tab2");
                    break;
                case 2:
                    VisualStateManager.GoToState(this, "Tab3");
                    break;
                case 3:
                    VisualStateManager.GoToState(this, "Tab4");
                    break;
            }
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshViews();
        }

        private async void RefreshViews()
        {
            ShowLoading = true;
            ShowNotConnected = false;
            ShowNotValidNetwork = false;
            ShowApp = false;

            var tasks = (App.Current as App).PlatformData.SetupTasks;
            bool hasTasks = false;
            foreach (var t in tasks)
            {
                if (!t.Done)
                    hasTasks = true;
                t.PropertyChanged += T_PropertyChanged;
            }
            HasSetupTasks = hasTasks;

            if (!hasTasks)
            {
                var ct = NativeAppHelper.Instance.GetSavedCredentials();
                pnlMainStatus.Refresh();
                if (ct == null)
                {
                    ShowNotConnected = true;
                }
                else
                {

                    var ssid = NativeAppHelper.Instance.GetNetworkName();
                    if (ssid == null)
                    {
                        ShowNotValidNetwork = true;
                    }
                    else
                    {
                        var found = false;
                        if (ct.AllowedSSIDs != null)
                        {
                            foreach (var t in ct.AllowedSSIDs)
                            {
                                if (t.Equals(ssid, StringComparison.InvariantCultureIgnoreCase))
                                    found = true;
                            }
                        }
                        if (!found)
                            ShowNotValidNetwork = true;
                        else
                            ShowApp = true;

                    }
                }
            }

            ShowLoading = false;
            OnPropertyChanged("HasSetupTasks");
            OnPropertyChanged("ShowLoading");
            OnPropertyChanged("ShowNotConnected");
            OnPropertyChanged("ShowNotValidNetwork");
            OnPropertyChanged("ShowApp");

            if (DateTime.Now.Hour <= 17)
            {
                lblGreetings1.Text = "Bonjour,";
                lblGreetings2.Text = "Bonjour,";
            }
            else
            {
                lblGreetings1.Text = "Bonsoir,";
                lblGreetings2.Text = "Bonsoir,";
            }

            RefreshTabs();

            if (ShowApp)
            {
                var gt = await new UserInfosBll().GetGreetings();
                if (gt != null && gt.Items != null)
                {
                    RefreshGreetings(gt);
                }
            }

        }

        private void RefreshGreetings(Home.Common.Messages.GreetingsMessageResponse gt)
        {
            pnlHome.Children.Clear();
            foreach (var t in gt.Items)
            {
                Label l = new Label();
                if (t.ContentKind == Home.Common.Messages.GreetingsMessageResponseItem.GreetingsMessageResponseItemKind.HeaderContent)
                    l.Style = Resources["HeaderLabelStyle"] as Style;
                else
                    l.Style = Resources["BlockLabelStyle"] as Style;
                var fs = new FormattedString();
                string[] contentParts = t.Content.Split(new string[] { "**" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < contentParts.Length; i++)
                {
                    var newSpan = new Span { Text = contentParts[i] };
                    if (i % 2 == 1)
                        newSpan.TextColor = Color.White;
                    fs.Spans.Add(newSpan);
                }
                l.FormattedText = fs;

                pnlHome.Children.Add(l);
            }
        }

        public int CurrentTab { get; set; }


        public bool ShowLoading { get; set; }
        public bool ShowNotConnected { get; set; }
        public bool ShowNotValidNetwork { get; set; }
        public bool ShowApp { get; set; }

        internal void RefreshPlatformData()
        {
            if (Dispatcher.IsInvokeRequired)
            {
                Dispatcher.BeginInvokeOnMainThread(new Action(DoRefreshPlatformData));
            }
            else
                DoRefreshPlatformData();
        }

        private void DoRefreshPlatformData()
        {
            var tasks = (App.Current as App).PlatformData.SetupTasks;
            bool hasTasks = false;
            foreach (var t in tasks)
            {
                if (!t.Done)
                    hasTasks = true;
                t.PropertyChanged += T_PropertyChanged;
            }
            HasSetupTasks = hasTasks;
            OnPropertyChanged("HasSetupTasks");
        }

        private void T_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tasks = (App.Current as App).PlatformData.SetupTasks;
            bool hasTasks = false;
            foreach (var t in tasks)
            {
                if (!t.Done)
                    hasTasks = true;
            }
            if (HasSetupTasks != hasTasks)
            {
                HasSetupTasks = hasTasks;
                OnPropertyChanged("HasSetupTasks");
            }
        }

        public bool HasSetupTasks { get; set; }

        private async void DoSetupTasksButton_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new SetupTasks());
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new LoginPage());
        }

        private async void btnRefreshNetwork_Clicked(object sender, EventArgs e)
        {
            try
            {
                RefreshViews();
                var coords = await NativeAppHelper.Instance.GetLocation();

                tmp.Text = $"Réseau à {coords?.Latitude.ToString("0.0000", CultureInfo.InvariantCulture)} , {coords?.Longitude.ToString("0.0000", CultureInfo.InvariantCulture)} inconnu";
            }
            catch(Exception ex)
            {

            }
        }
    }
}
