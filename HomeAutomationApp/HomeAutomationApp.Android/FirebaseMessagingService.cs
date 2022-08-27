using Android.Util;
using Firebase.Messaging;
using Android.Support.V4.App;
using WindowsAzure.Messaging;
using Android.App;
using Android.Content;
using System.Linq;
using System.Collections.Generic;
using System;
using Android.Graphics;
using Android.OS;
using Newtonsoft.Json;
using System.Text;
using Home.Common.Messages;

namespace HomeAutomationApp.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {

        public MyFirebaseMessagingService()
        {
            _instance = this;
        }



        internal static Context _instance = null;

        const string TAG = "MyFirebaseMsgService";
        public static NotificationHub hub;

        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);
            if (message.GetNotification() != null)
            {
                //These is how most messages will be received
                Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
                SendNotification(message, message.GetNotification());


            }
            else
            {
                //Only used for debugging payloads sent from the Azure portal
                //SendNotification(message.Data.Values.First(), "Test");
                var messageType = "";
                if (!message.Data.TryGetValue("messageType", out messageType))
                    return;
                if (string.IsNullOrEmpty(messageType))
                    messageType = "";

                switch(messageType.ToLowerInvariant())
                {
                    case "greetings_change":
                        Xamarin.Forms.MessagingCenter.Send("Notification", messageType, message.Data); 
                        break;
                }

            }
        }

        private void SendNotification(RemoteMessage message, RemoteMessage.Notification notification)
        {

            string messageBody = notification.Body;
            string title = notification.Title;
            int priorite = message.Priority;
            
            //CentralizedLog.Log("Reçu la notification : " + title + ", " + messageBody, false);
            
            string vibrate = "false";
            if (!message.Data.TryGetValue("vibrate", out vibrate))
                vibrate = "false";

            var intent = new Intent(this, typeof(MainActivity));

            string category = "default";
            if (!message.Data.TryGetValue("custom_category", out category))
                category = "default";

            foreach (var r in message.Data.Keys)
            {
                Log.Debug("FIREBASEID", "Recu le message : " + r + "=" + message.Data[r]);
                intent.PutExtra(r, message.Data[r]);
            }
            intent.AddFlags(ActivityFlags.SingleTop | ActivityFlags.NewTask);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            //var stackBuilder = Android.App.TaskStackBuilder.Create(this);
            //stackBuilder.AddNextIntentWithParentStack(intent);

            string channel_id = SendMobileNotificationMessage.MAIN_CHANNEL_ID;
            switch(category.ToLowerInvariant())
            {
                case "default":
                    break;
                case "alert":
                case "alerts":
                    channel_id = SendMobileNotificationMessage.ALERTS_CHANNEL_ID;
                    break;
                case "messages":
                case "personal":
                    channel_id = SendMobileNotificationMessage.PERSONAL_CHANNEL_ID;
                    break;
                case "chat":
                    channel_id = SendMobileNotificationMessage.CHAT_CHANNEL_ID;
                    break;
                case "downloads":
                    channel_id = SendMobileNotificationMessage.DOWNLOADS_CHANNEL_ID;
                    break;
            }

            category = category.ToLowerInvariant();

            var notificationBuilder = new NotificationCompat.Builder(this, channel_id);

            StringBuilder blr = new StringBuilder();
            string[] contentParts = messageBody.Split(new string[] { "**" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < contentParts.Length; i++)
            {
                if (i % 2 == 1)
                    blr.Append("<strong>");
                blr.Append(contentParts[i]);
                if (i % 2 == 1)
                    blr.Append("</strong>");
            }
            messageBody = blr.ToString();
            blr.Clear();
            
            contentParts = title.Split(new string[] { "**" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < contentParts.Length; i++)
            {
                if (i % 2 == 1)
                    blr.Append("<strong>");
                blr.Append(contentParts[i]);
                if (i % 2 == 1)
                    blr.Append("</strong>");
            }
            title = blr.ToString();


            notificationBuilder.SetContentTitle(title)
                        .SetSmallIcon(Resource.Drawable.ic_notify)
                        //.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Mipmap.notificon))
                        .SetContentText(messageBody)
                        .SetAutoCancel(true)
                        .SetPriority(priorite)
                        .SetShowWhen(true)
                        .SetContentIntent(pendingIntent);

            bool bVibrate;
            if (bool.TryParse(vibrate, out bVibrate))
            {
                if (bVibrate)
                    notificationBuilder.SetVibrate(new long[] { 150, 250, 50, 250 });
            }


            switch (category)
            {
                case "default":
                    break;
                case "downloads":
                    notificationBuilder.SetGroup("downloads");
                    break;
                case "alert":
                case "alerts":
                    notificationBuilder.SetGroup("Alerts");
                    break;
                case "messages":
                case "personal":
                    notificationBuilder.SetGroup("Personal");
                    break;
                case "chat":
                    notificationBuilder.SetGroup("Chat");
                    break;
            }


            //if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            //{
            //    try
            //    {
            //        MainActivity.CreateNotificationChannel(this);
            //        NotificationManager notManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
            //        notManager.Notify(0, notificationBuilder.Build());
            //    }
            //    catch
            //    {

            //    }
            //}
            //else
            //{
            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(1, notificationBuilder.Build());
            //}

        }

        public override void OnNewToken(string token)
        {
            Log.Debug(TAG, "FCM token: " + token);
            DroidNativeHelper.Instance.SaveAzureNotificationToken(token);
            SendRegistrationToServer(token);
        }

        void SendRegistrationToServer()
        {
            string token = DroidNativeHelper.Instance.GetAzureNotificationToken();
            SendRegistrationToServer(token);
        }

        public static void SendRegistrationToServer(string token)
        {
            var cred = DroidNativeHelper.Instance.GetSavedCredentials();
            SendRegistrationToServer(token, cred);
        }



        public static void SendRegistrationToServer(string token, NativeAppHelper.Credentials cred)
        {

            if (token == null)
                return;

            // Register with Notification Hubs
            if (hub == null)
                hub = new NotificationHub(MainActivity.NotificationHubName,
                                        MainActivity.ListenConnectionString, _instance);

            try
            {
                hub.UnregisterAll(token);
            }
            catch
            {

            }

            if (cred == null)
                return;
            if (cred.MeshId == null)
                return;


            var uxid = Guid.Parse(cred.MeshId);

            var tags = new List<string>() {
                "mesh_" + uxid.ToString("N").ToUpperInvariant(),
                "user_" + cred.UserId.ToLowerInvariant()
#if DEBUG
                , "debug"
#endif
            };

            var regID = hub.Register(token, tags.ToArray()).RegistrationId;
            string lesTags = string.Join(",", tags);
            Log.Debug(TAG, $"Successful registration of ID {regID} pour {lesTags}");
            //CentralizedLog.Log("Ajouté un abonnement aux notifications pour " + lesTags + " : " + regID, false);
        }
    }
}