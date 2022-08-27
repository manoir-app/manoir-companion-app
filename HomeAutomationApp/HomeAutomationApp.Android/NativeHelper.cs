using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Home.Common.Model;
using Xamarin.Essentials;

namespace HomeAutomationApp.Droid
{
    public class DroidNativeHelper : NativeAppHelper
    {
        public override string GetNetworkName()
        {
            WifiManager mgr = (WifiManager)_ctx.GetSystemService(Context.WifiService);

            if (mgr != null)
            {
                if (mgr.IsWifiEnabled)
                {
                    var cni = mgr.ConnectionInfo;
                    if (cni != null)
                    {
                        string s = cni.SSID;
                        if (s.StartsWith("\""))
                            s = s.Substring(1);
                        if (s.EndsWith("\""))
                            s = s.Substring(0, s.Length - 1);
                        return s;
                    }
                }
            }

            return null;
        }


        protected override void OnCredentialsChanged(Credentials c)
        {
            //AppCenter.SetUserId(c.Uxid);
            MyFirebaseMessagingService.SendRegistrationToServer(GetAzureNotificationToken(), c);
            GeoFencingHelper.RefreshList().Wait();
        }

        private Context _ctx;

        public DroidNativeHelper(Context ctx)
        {
            _ctx = ctx;
            NativeAppHelper._instance = this;
            preferences = ctx.GetSharedPreferences("myapp.settings", FileCreationMode.Private);
            var f = ctx.GetDir("data", FileCreationMode.Private);
            commonDir = f.AbsolutePath;
        }



        public override string GetVersion()
        {
            var context = Android.App.Application.Context;
            return context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;
        }

        private string commonDir;
        private ISharedPreferences preferences;
        private const string initVector = "altazion--office";
        private const string passPhrase = "thisisalongphraseusedtogetrandomthings";
        private const int keysize = 256;

        protected override string GetString(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (!preferences.Contains(key))
            {
                return null;
            }

            return DecryptString(preferences.GetString(key, string.Empty));
            //return null;
        }

        protected override void SaveString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var editor = preferences.Edit();
            if (value == null)
            {
                if (preferences.Contains(key))
                    editor.Remove(key);
            }
            else
            {
                var encryptedValue = EncryptString(value);
                editor.PutString(key, encryptedValue);
            }
            editor.Apply();
        }

        private string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        private string DecryptString(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }


        public override async Task<GeoCoordinates> GetLocation()
        {
            var fusedLocationClient = LocationServices.GetFusedLocationProviderClient(_ctx);

            var val = await fusedLocationClient.GetLastLocationAsync();
            if (val == null)
                return new GeoCoordinates()
                {
                };

            return new GeoCoordinates()
            {
                Latitude = (decimal)val.Latitude,
                Longitude = (decimal)val.Longitude
            };
        }

        public override Stream OpenRead(string relativeFilePath)
        {
            string pth = Path.Combine(commonDir, relativeFilePath);
            return File.OpenRead(pth);
        }

        public override Stream OpenWrite(string relativeFilePath)
        {
            string pth = Path.Combine(commonDir, relativeFilePath);
            return File.OpenWrite(pth);
        }

        public override bool FileExists(string relativeFilePath)
        {
            string pth = Path.Combine(commonDir, relativeFilePath);
            return File.Exists(pth);
        }
    }
}