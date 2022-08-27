
using HomeAutomationApp.Business;
using System;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeAutomationApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainStatus : ContentView
    {
        public MainStatus()
        {
            InitializeComponent();
            Refresh();

            t.Elapsed += T_Elapsed;
            t.Start();

            lblOkNetwork.Source = ImageSource.FromResource("HomeAutomationApp.Images.streamline-icon-wifi_140.png", typeof(MainPage).Assembly);
        }

        Timer t = new Timer(1500);

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool isOk = false;

            var ssid = NativeAppHelper.Instance.GetNetworkName();
            if (ssid == null)
            {
                isOk = false;
            }
            else
            {
                var ct = NativeAppHelper.Instance.GetSavedCredentials();
                var found = false;

                if (ct.AllowedSSIDs != null)
                {
                    foreach (var t in ct.AllowedSSIDs)
                    {
                        if (t.Equals(ssid, StringComparison.InvariantCultureIgnoreCase))
                            found = true;
                    }
                }

                UserInfosBll.SetOnMesh(found);

                isOk = found;
            }

            Dispatcher.BeginInvokeOnMainThread(new Action(() =>
           {
               if (isOk)
               {
                   lblOkNetwork.Opacity = 1;
               }
               else
               {
                   lblOkNetwork.Opacity = 0.2;
               }
           }));
        }

        public void Refresh()
        {
            var ct = NativeAppHelper.Instance.GetSavedCredentials();
            string userId = ct?.UserId;
            if (userId == null)
            {
                lblUserSmall.Text = "-";
                lblUserSmall.Opacity = 0.2;
                return;
            }


            if (userId.Length > 2)
                userId = userId.Substring(0, 2);


            userId = userId.ToUpperInvariant();

            lblUserSmall.Text = userId;
            lblUserSmall.Opacity = 1;
        }

    }
}