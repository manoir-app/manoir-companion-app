using Home.Common.Messages;
using Home.Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HomeAutomationApp.Business
{
    public class UserInfosBll : BaseBll
    {
        private static DateTime _lastRefresh = DateTime.MinValue;

        public async Task<GreetingsMessageResponse> GetGreetings()
        {
            if (_lastRefresh.AddHours(2) > DateTime.Now)
            {
                if (NativeAppHelper.Instance.FileExists("userinfos_greetings.json"))
                {
                    using (var st = NativeAppHelper.Instance.OpenRead("userinfos_greetings.json"))
                    using (var rdr = new StreamReader(st))
                    {
                        var val = await rdr.ReadToEndAsync();
                        var ret = JsonConvert.DeserializeObject<GreetingsMessageResponse>(val);
                        if (ret != null)
                            return ret;
                    }
                }
            }

            return await RefreshGreetings();
        }

        private async Task<GreetingsMessageResponse> RefreshGreetings()
        {
            try
            {
                var url = $"/v1.0/users/interactions/greetings/fromdevice";
                var tmp = await DownloadData<GreetingsMessageResponse>(url);

                return await RefreshGreetings(tmp);
            }
            catch
            {
                return null;
            }
        }

        private static async Task<GreetingsMessageResponse> RefreshGreetings(GreetingsMessageResponse tmp)
        {
            _lastRefresh = DateTime.Now;

            if (tmp != null)
            {
                using (var st = NativeAppHelper.Instance.OpenWrite("userinfos_greetings.json"))
                using (var rdr = new StreamWriter(st))
                {
                    await rdr.WriteAsync(JsonConvert.SerializeObject(tmp));
                }
            }

            await NotifyUserActivity("greetingsrefresh");

            return tmp;
        }

        public async Task<List<User>> GetMainUsers()
        {
            try
            {
                var url = $"/v1.0/users/main";
                var tmp = await DownloadData<List<User>>(url);

                return tmp;
            }
            catch
            {
                return null;
            }
        }


        private static bool _onMesh = false;
        public static void SetOnMesh(bool onMesh)
        {
            _onMesh = onMesh;
        }

        public static async Task NotifyUserActivity(string status)
        {
            if (!_onMesh)
                return;

            var t = NativeAppHelper.Instance.GetSavedCredentials();
            if (t == null) return;
            var upd = new PresenceNotificationData()
            {
                AssociatedUser = t.UserId,
                ActivityKind = "mobileappusage",
                DeviceId = t.DeviceId,
                Date = DateTimeOffset.Now,
                IsUserInput = true,
                Status = status
            };

            try
            {
                var url = $"/v1.0/users/presence/notifyactivity";
                var tmp = await new UserInfosBll().UploadData<bool>(url, upd);
            }
            catch
            {

            }

        }
    }
}
