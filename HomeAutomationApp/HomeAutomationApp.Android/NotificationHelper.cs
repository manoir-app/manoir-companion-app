using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Home.Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomationApp.Droid
{
    public static class NotificationHelper
    {

        public static void RegisterCategories(Context ctx)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var notificationManager = (NotificationManager)ctx.GetSystemService(Activity.NotificationService);


            var channel = new NotificationChannel(SendMobileNotificationMessage.MAIN_CHANNEL_ID, "Main notifications", NotificationImportance.Default)
            {
                Description = "Les notifications principales de l'application",
            };
            notificationManager.CreateNotificationChannel(channel);


            channel = new NotificationChannel(SendMobileNotificationMessage.DOWNLOADS_CHANNEL_ID, "Downloads", NotificationImportance.Low)
            {
                Description = "Téléchargements & news",
            };
            notificationManager.CreateNotificationChannel(channel);

            channel = new NotificationChannel(SendMobileNotificationMessage.PERSONAL_CHANNEL_ID, "Personal", NotificationImportance.Default)
            {
                Description = "Messages personnels",
            };
            notificationManager.CreateNotificationChannel(channel);

            channel = new NotificationChannel(SendMobileNotificationMessage.ALERTS_CHANNEL_ID, "Alerts", NotificationImportance.High)
            {
                Description = "Alertes"
            };
            notificationManager.CreateNotificationChannel(channel);

            channel = new NotificationChannel(SendMobileNotificationMessage.CHAT_CHANNEL_ID, "Chat", NotificationImportance.Default)
            {
                Description = "Chat"
            };
            notificationManager.CreateNotificationChannel(channel);

        }

    }
}