using Home.Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeAutomationApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        Color _defaultTextColor;
        Color _defaultPlaceholderColor;


        private class LoginWebClient : WebClient
        {
            
            protected override WebRequest GetWebRequest(Uri address)
            {
                var t = base.GetWebRequest(address);
                
               if(t is HttpWebRequest)
                {
                    var h = t as HttpWebRequest;
                    h.ServicePoint.BindIPEndPointDelegate = (servicePoint, remoteEndPoint, retryCount) =>
                    {
                        if (remoteEndPoint.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return new IPEndPoint(IPAddress.Any, 0);
                        }

                        throw new InvalidOperationException("no IPv6 address");
                    };
                }

                return t;
            }
        }

        public LoginPage()
        {
            InitializeComponent();

            ServicePointManager
            .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _defaultTextColor = txtServer.TextColor;
            _defaultPlaceholderColor = txtServer.PlaceholderColor;
        }

        private async void bntLogin_Clicked(object sender, EventArgs e)
        {
            bool err = false;
            string srv = txtServer.Text;
            if (string.IsNullOrEmpty(srv))
            {
                txtServer.PlaceholderColor = Color.Firebrick;
                err = true;
            }
            string usr = txtUser.Text;
            if (string.IsNullOrEmpty(usr))
            {
                txtUser.PlaceholderColor = Color.Firebrick;
                err = true;
            }

            string pwd = txtPassword.Text;
            if (string.IsNullOrEmpty(pwd))
            {
                txtPassword.PlaceholderColor = Color.Firebrick;
                err = true;
            }

            if (err)
                return;

            frmSaisie.IsEnabled = false;
            frmSaisie.Opacity = 0.75;
            try
            {
                using (var cli = new LoginWebClient())
                {
                    string publicSrv = null;

                    Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                    {
                        lblStatus.Text = "Connecting to server";
                    }));

                    string protocol = "http";

                    if(srv.EndsWith(".manoir.app") || srv.EndsWith(".manoir.dev"))
                        protocol = "https";

                    var strret = await cli.DownloadStringTaskAsync($"{protocol}://{srv}/v1.0/system/mesh/local/graph/check");
                    if (strret.Equals("dev.carbenay.info:home-automation:proxy"))
                    {
                        Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                        {
                            lblStatus.Text = "Server is a proxy, getting local name";
                        }));

                        strret = await cli.DownloadStringTaskAsync($"{protocol}://{srv}/v1.0/system/mesh/local/graph/localip");
                        if (!string.IsNullOrEmpty(strret))
                        {
                            publicSrv = srv;
                            srv = strret;
                            protocol = "http";
                            Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                            {
                                lblStatus.Text = "Logging in to local server";
                            }));
                        }
                    }
                    else
                        Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                        {
                            lblStatus.Text = "Logging in";
                        }));


                    var machineId = NativeAppHelper.Instance.GetAzureNotificationToken();
                    if (string.IsNullOrEmpty(machineId))
                        machineId = Environment.MachineName;

                    cli.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    var req = new LoginFromDeviceRequest()
                    {
                        deviceKind = "mobiledevice",
                        deviceInternalName = machineId,
                        login = usr,
                        pwd = pwd
                    };

                    var ret = await cli.UploadStringTaskAsync($"{protocol}://{srv}/v1.0/users/login/device?associateWithUser=true",
                        "POST",
                        JsonConvert.SerializeObject(req));
                    var res = JsonConvert.DeserializeObject<LoginFromDeviceResponse>(ret);
                    if (res != null && res.User != null)
                    {
                        Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                        {
                            lblStatus.Text = "Logged in";
                        }));

                        var cred = new NativeAppHelper.Credentials()
                        {
                            ServerUrl = $"{protocol}://{srv}/",
                            DeviceId = res.Device.Id,
                            Password = res.DeviceApiKey,
                            MeshId = res.Mesh.PublicId,
                            UserId = res.User.Id
                        };
                        if (!string.IsNullOrEmpty(publicSrv))
                            cred.PublicServerUrl = $"http://{publicSrv}/";

                        cred.AllowedSSIDs = new List<string>();

                        Thread t = new Thread(() =>
                        {
                            NativeAppHelper.Instance.SaveCredentials(cred);
                        });
                        t.Start();
                        Dispatcher.BeginInvokeOnMainThread(new Action(async () =>
                        {
                            Thread.Sleep(200);
                            await this.Navigation.PopAsync();
                        }));
                    }
                    else
                    {
                        Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                        {
                            lblStatus.Text = "Login failed !";
                            frmSaisie.IsEnabled = true;
                            txtPassword.Focus();
                            frmSaisie.Opacity = 1;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                {
                    frmSaisie.IsEnabled = true;
                    frmSaisie.Opacity = 1;
                }));
            }

        }


        public class LoginFromDeviceRequest
        {
            public string login { get; set; }
            public string pwd { get; set; }

            public string deviceInternalName { get; set; }
            public string deviceKind { get; set; }
        }
        public class LoginFromDeviceResponse
        {
            public User User { get; set; }
            public Home.Common.Model.Device Device { get; set; }
            public AutomationMesh Mesh { get; set; }
            public string DeviceApiKey { get; set; }
        }

        private async void txtServer_Unfocused(object sender, FocusEventArgs e)
        {

            string srv = txtServer.Text;
            if (string.IsNullOrEmpty(srv))
            {

            }
            else
            {
                try
                {
                    var host = Dns.GetHostEntry(srv);

                    if(host==null)
                    {
                        Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                        {
                            txtServer.TextColor = _defaultTextColor;
                        }));
                        return;
                    }

                    string protocol = "http";

                    if (srv.EndsWith(".manoir.app") || srv.EndsWith(".manoir.dev"))
                        protocol = "https";


                    using (var cli = new LoginWebClient())
                    {
                        var ret = await cli.DownloadStringTaskAsync($"{protocol}://{srv}/v1.0/system/mesh/local/graph/check");
                        if (ret.Equals("dev.carbenay.info:home-automation:graph"))
                        {
                            Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                           {
                               txtServer.TextColor = _defaultTextColor;
                           }));
                        }
                        else if (ret.Equals("dev.carbenay.info:home-automation:proxy"))
                        {
                            Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                            {
                                txtServer.TextColor = _defaultTextColor;
                            }));
                        }
                        else
                        {
                            Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                            {
                                txtServer.TextColor = Color.Firebrick;
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    Dispatcher.BeginInvokeOnMainThread(new Action(() =>
                    {
                        txtServer.TextColor = Color.Firebrick;
                    }));
                }
            }
        }

        private void txtServer_Focused(object sender, FocusEventArgs e)
        {
            txtServer.TextColor = _defaultTextColor;
        }
    }
}