
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using AndroidX.AppCompat.App;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using System;
using System.Threading;

namespace HomeAutomationApp.Droid
{
    [Activity(Label = "maNoir", Icon = "@mipmap/icon", MainLauncher = true, Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
              Categories = new[] { Android.Content.Intent.CategoryBrowsable, Android.Content.Intent.CategoryDefault },
              DataScheme = "ma-noir")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private static NativeAppHelper _nativeHelper = null;

        public const string TAG = "MainActivity";
        internal static MainActivity Instance { get; private set; }

        public const string ListenConnectionString = "Endpoint=sb://home-automation-hemcefor.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=HMUoqhJuBb404erpzN3aW51S8QEDJoI9dO2NTRRdw+8=";
        public const string NotificationHubName = "home-automation";

        private App _theApp = null;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            //RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);


            //int uiOptions = (int)Window.DecorView.SystemUiVisibility;

            //uiOptions |= (int)SystemUiFlags.LowProfile;
            //uiOptions |= (int)SystemUiFlags.Fullscreen;
            ////uiOptions |= (int)SystemUiFlags.HideNavigation;
            //uiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            //Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;

            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;

            if (_nativeHelper == null)
                _nativeHelper = new DroidNativeHelper(this.ApplicationContext);
            MyFirebaseMessagingService._instance = this.ApplicationContext;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            NotificationHelper.RegisterCategories(this);

            //Window.DecorView.SystemUiVisibility = Android.Views.StatusBarVisibility.Hidden;


            this.Window.SetStatusBarColor(Color.ParseColor("#343E7A"));

            Xamarin.Forms.Forms.SetFlags("Brush_Experimental");

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            _theApp = new App();

            Distribute.SetEnabledForDebuggableBuild(true);
            AppCenter.Start("1269f8ce-5d1b-4659-92aa-2c6880727c38", typeof(Distribute), typeof(Crashes), typeof(Analytics));

            LoadApplication(_theApp);

            //Window.AddFlags(Android.Views.WindowManagerFlags.Fullscreen);


            var app = (App.Current as App);

            app.StartupTask += App_StartupTask;
            var setPhone = new Model.PlatformSetupTask()
            {
                Code = Manifest.Permission.AnswerPhoneCalls,
                Description = "Donnez l'accès à la partie téléphonie",
                Name = "Accès téléphonie"
            };
            var pm = ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AnswerPhoneCalls);
            if (pm == Permission.Granted)
                setPhone.Done = true;
            setPhone.SetImage("streamline-icon-phone_140.png");

            var setSms = new Model.PlatformSetupTask()
            {
                Code = Manifest.Permission.WriteSms,
                Description = "Donnez l'accès à la partie sms",
                Name = "Accès Sms"
            };
            pm = ActivityCompat.CheckSelfPermission(this, Manifest.Permission.SendSms);
            if (pm == Permission.Granted)
                setSms.Done = true;
            setSms.SetImage("streamline-icon-messages-bubble-square_140.png");

            var setNetwork = new Model.PlatformSetupTask()
            {
                Code = Manifest.Permission.AccessNetworkState,
                Description = "Donnez l'accès à la partie network",
                Name = "Accès Networks"
            };
            pm = ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessNetworkState);
            if (pm == Permission.Granted)
                setNetwork.Done = true;
            setNetwork.SetImage("streamline-icon-server-1_140.png");

            var setFineLoc = new Model.PlatformSetupTask()
            {
                Code = Manifest.Permission.AccessFineLocation,
                Description = "Donnez l'accès à la localisation",
                Name = "Accès Localisation"
            };
            pm = ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);
            if (pm == Permission.Granted)
                setFineLoc.Done = true;
            setFineLoc.SetImage("streamline-icon-pin_140.png");

            var setCalendar = new Model.PlatformSetupTask()
            {
                Code = Manifest.Permission.ReadCalendar,
                Description = "Donnez l'accès à vos calendriers",
                Name = "Accès calendrier"
            };
            pm = ActivityCompat.CheckSelfPermission(this, Manifest.Permission.ReadCalendar);
            if (pm == Permission.Granted)
                setCalendar.Done = true;
            setCalendar.SetImage("streamline-icon-calendar_140.png");

            var setContact = new Model.PlatformSetupTask()
            {
                Code = Manifest.Permission.ReadContacts,
                Description = "Donnez l'accès à vos contacts",
                Name = "Accès contacts"
            };
            setContact.SetImage("streamline-icon-multiple-users-1_140.png");
            pm = ActivityCompat.CheckSelfPermission(this, Manifest.Permission.ReadContacts);
            if (pm == Permission.Granted)
                setContact.Done = true;

            app.SetPlatformData(new Model.PlatformData()
            {
                PlatformaName = "Android",
                Version = "0",
                SetupTasks = new Model.PlatformSetupTask[]
                {
                    setPhone, setSms,
                    setNetwork, setFineLoc,

                    setCalendar,setContact
                }
            });

            Distribute.CheckForUpdate();

            Thread t = new Thread(async () =>
           {
               if (NativeAppHelper.Instance.GetSavedCredentials() != null)
               {
                   await GeoFencingHelper.RefreshList(this);
               }
           });
            t.Start();


            if (Intent != null && Intent.DataString != null)
            {
                try
                {
                    //Xamarin.Forms.MessagingCenter.Send<string, string>("", "DeepLink", intent.DataString);
                    _theApp.OnAppLinkReceived(Intent.DataString);
                }
                catch (Exception e)
                {
                    //Catch error
                }
            }
        }



        private void App_StartupTask(object sender, StartupTaskEventArgs e)
        {
            string[] permissions = null;

            switch (e.StartupTaskCode)
            {
                case Manifest.Permission.AnswerPhoneCalls:
                    permissions = new string[]
                    {
                        Manifest.Permission.AnswerPhoneCalls,
                        Manifest.Permission.ReadPhoneState,
                        Manifest.Permission.ReadCallLog,
                    };
                    break;
                case Manifest.Permission.WriteSms:
                    permissions = new string[]
                    {
                        Manifest.Permission.SendSms,
                        Manifest.Permission.ReadSms,
                        Manifest.Permission.WriteSms,
                    };
                    break;
                case Manifest.Permission.AccessFineLocation:
                    permissions = new string[]
                    {
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation,
                        Manifest.Permission.AccessMockLocation,
                        Manifest.Permission.AccessLocationExtraCommands,
                        "android.permission.ACCESS_BACKGROUND_LOCATION"
                    };
                    break;
                case Manifest.Permission.ReadCalendar:
                    permissions = new string[]
                    {
                        Manifest.Permission.ReadCalendar
                    };
                    break;
                case Manifest.Permission.ReadContacts:
                    permissions = new string[]
                    {
                        Manifest.Permission.ReadContacts,
                    };
                    break;
            }

            ActivityCompat.RequestPermissions(this, permissions, 123);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            var app = (App.Current as App);
            if (app.PlatformData == null)
                return;

            for (int i = 0; i < permissions.Length; i++)
            {
                if (grantResults[i] == Permission.Granted)
                {
                    foreach (var t in app.PlatformData.SetupTasks)
                    {
                        if (t.Code.Equals(permissions[i]))
                            t.Done = true;
                    }
                }
            }

        }
    }
}