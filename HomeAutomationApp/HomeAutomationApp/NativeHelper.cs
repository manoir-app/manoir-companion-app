using Home.Common.Model;
using HomeAutomationApp.Business;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace HomeAutomationApp
{
    public abstract class NativeAppHelper
    {
        protected NativeAppHelper()
        {

        }

        protected static NativeAppHelper _instance = null;

        public static NativeAppHelper Instance { get { return _instance; } }

        public class Credentials
        {
            public string ServerUrl { get; set; }
            public string PublicServerUrl { get; set; }
            public string DeviceId { get; set; }
            public string Password { get; set; }
            public string MeshId { get; set; }
            public string UserId { get; set; }

            public List<string> AllowedSSIDs { get; set; }
        }


        public bool GetSecureModeEnabled()
        {
            string s = GetString("IsSecureModeEnabled");
            bool retVal = false;
            if (bool.TryParse(s, out retVal))
                return retVal;

            return false;
        }

        public void SetSecureModeEnabled(bool isSecureModeEnabled)
        {
            SaveString("IsSecureModeEnabled", isSecureModeEnabled.ToString());
        }


        public string GetAzureNotificationToken()
        {
            string s = GetString("AzureNotificationToken");
            return s;
        }
        public void SaveAzureNotificationToken(string token)
        {
            SaveString("AzureNotificationToken", token);
        }

        public Credentials GetSavedCredentials()
        {
            string s = GetString("Credentials");
            if (string.IsNullOrEmpty(s))
                return null;

            return JsonConvert.DeserializeObject<Credentials>(s);
        }



        public void SaveCredentials(Credentials c)
        {
            var s = JsonConvert.SerializeObject(c);
            SaveString("Credentials", s);
            try
            {
                var ic = new MeshBll().Get().Result;
                c.AllowedSSIDs = new List<string>();
                c.AllowedSSIDs.Add(ic.MainSsid);
                s = JsonConvert.SerializeObject(c);
                SaveString("Credentials", s);
                //Analytics.TrackEvent("Login");
                //CentralizedLog.Log("Login:RjsId" + c?.RjsId + "/Uxid:" + c?.Uxid.ToLower(), false);
            }
            catch
            {
                c.AllowedSSIDs = new List<string>();
                c.AllowedSSIDs.Add("Maison_Hemce");
                s = JsonConvert.SerializeObject(c);
                SaveString("Credentials", s);
            }
            OnCredentialsChanged(c);
        }

        public abstract Stream OpenRead(string relativeFilePath);
        public abstract Stream OpenWrite(string relativeFilePath);
        public abstract bool FileExists(string relativeFilePath);


        public void ClearCredentials()
        {
            SaveString("Credentials", null);
            OnCredentialsChanged(null);
        }

        public void SaveBaseUrl(string baseUrl)
        {
            if (null == baseUrl)
                baseUrl = "";

            SaveString("BaseUrl", baseUrl);
        }

        public string GetBaseUrl()
        {
            return GetString("BaseUrl");
        }

        protected virtual void OnCredentialsChanged(Credentials c)
        {

        }

        public abstract string GetNetworkName();

        protected abstract string GetString(string key);

        protected abstract void SaveString(string key, string value);

        public abstract string GetVersion();


        public abstract Task<GeoCoordinates> GetLocation();

    }
}
