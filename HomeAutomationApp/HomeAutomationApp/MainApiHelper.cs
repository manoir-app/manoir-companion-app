using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HomeAutomationApp
{
    public class MainApiHelper
    {

        public static T Get<T>(string url) where T : class
        {
            using (var cli = new WebClient())
            {
                cli.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                var cred = NativeAppHelper.Instance.GetSavedCredentials();
                cli.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + cred.Password);
                cli.BaseAddress = cred.ServerUrl;

                var json = cli.DownloadString(url);
                if (json != null)
                    return JsonConvert.DeserializeObject<T>(json);
            }

            return null;
        }
    }
}
