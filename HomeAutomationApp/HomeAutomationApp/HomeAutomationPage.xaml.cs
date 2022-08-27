using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeAutomationApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeAutomationPage : ContentPage
    {
        public HomeAutomationPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            //imgFond.Source = ImageSource.FromResource("HomeAutomationApp.Images.fond-house.png", typeof(MainPage).Assembly);
            imgScenes.Source = ImageSource.FromResource("HomeAutomationApp.Images.streamline-icon-single-man-circle.png", typeof(MainPage).Assembly);
            imgHouse.Source = ImageSource.FromResource("HomeAutomationApp.Images.streamline-icon-real-estate-search-house-1.png", typeof(MainPage).Assembly);
            imgDevices.Source = ImageSource.FromResource("HomeAutomationApp.Images.streamline-icon-messages-bubble-typing-1.png", typeof(MainPage).Assembly);
            imgUsers.Source = ImageSource.FromResource("HomeAutomationApp.Images.streamline-icon-style-three-pin-check.png", typeof(MainPage).Assembly);

            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += btnScenes_Tapped;
            btnScenes.GestureRecognizers.Add(tapGesture);
            tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += btnHouse_Tapped;
            btnHouse.GestureRecognizers.Add(tapGesture);
            tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += btnUsers_Tapped;
            btnUsers.GestureRecognizers.Add(tapGesture);
            tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += btnDevices_Tapped;
            btnDevices.GestureRecognizers.Add(tapGesture);

            RefreshTabs();

            tRefreshData.Elapsed += RefreshDataTimer_Elapsed;
            tRefreshData.Interval = 15000;
            tRefreshData.AutoReset = false;

            lstScenes.ItemsSource = Groups;
            lstDevices.ItemsSource = Devices;
        }
        
        Timer tRefreshData = new Timer();

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            tRefreshData.Start();
            await RefreshData();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            tRefreshData.Stop();
        }

        private async  void RefreshDataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await RefreshData();
            tRefreshData.Start();
        }

        private async Task RefreshData()
        {
            IsLoading = true;
            OnPropertyChanged("IsLoading");

            var bll = new Business.HomeAutomationBll();
            var scenes = await bll.GetGroupsAndScenes();

            Groups = scenes;
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                lstScenes.ItemsSource = Groups;
            });
            OnPropertyChanged("Groups");

            var devices = await bll.GetAllDevices();

            Devices = devices;
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                lstDevices.ItemsSource = devices;
            });
            OnPropertyChanged("Devices");


            IsLoading = false;
            OnPropertyChanged("IsLoading");
        }

        public List<Business.SceneGroupDetails> Groups = new List<Business.SceneGroupDetails>();
        public List<Business.DeviceWithDetails> Devices = new List<Business.DeviceWithDetails>();
        public bool IsLoading { get; set; }
        public int CurrentTab { get; set; }

        private void btnDevices_Tapped(object sender, EventArgs e)
        {
            CurrentTab = 3;
            OnPropertyChanged("CurrentTab");

            RefreshTabs();
        }
        private void btnUsers_Tapped(object sender, EventArgs e)
        {
            CurrentTab = 2;
            OnPropertyChanged("CurrentTab");

            RefreshTabs();
        }

        private void btnScenes_Tapped(object sender, EventArgs e)
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

        protected override bool OnBackButtonPressed()
        {
            if (btnMainMenu.HandleBack())
                return true;

            return base.OnBackButtonPressed();
        }

        private async void btnActivateScene_Clicked(object sender, EventArgs e)
        {
            tRefreshData.Stop();
            var btn = sender as Button;
            var scene = btn.BindingContext as Business.SceneWithState;
            scene.IsActivating = true;
            await new Business.HomeAutomationBll().ActivateScene(scene.Id);
            await Task.Delay(2500);
            await RefreshData();
            tRefreshData.Start();

        }
    }
}