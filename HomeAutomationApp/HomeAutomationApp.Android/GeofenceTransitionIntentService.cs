using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Support.V4.App;
using Android.Util;
using Android.Widget;
using System.Collections.Generic;


namespace HomeAutomationApp.Droid
{
    //[Service]
    //public class GeofenceTransitionsIntentService : IntentService
    //{
    //    private const string TAG = "GeofenceTransitionsIS";

    //    public GeofenceTransitionsIntentService() : base(TAG)
    //    {
    //    }

    //    protected override void OnHandleIntent(Intent intent)
    //    {
    //        var geofencingEvent = GeofencingEvent.FromIntent(intent);
    //        if (geofencingEvent.HasError)
    //        {
    //            var errorMessage = "Erreur";
    //            Log.Error(TAG, errorMessage);
    //            return;
    //        }

    //        int geofenceTransition = geofencingEvent.GeofenceTransition;

    //        if (geofenceTransition == Geofence.GeofenceTransitionEnter ||
    //            geofenceTransition == Geofence.GeofenceTransitionExit)
    //        {

    //            IList<IGeofence> triggeringGeofences = geofencingEvent.TriggeringGeofences;

    //            string geofenceTransitionDetails = GetGeofenceTransitionDetails(this, geofenceTransition, triggeringGeofences);

    //            SendNotification(geofenceTransitionDetails);
    //            Log.Info(TAG, geofenceTransitionDetails);
    //        }
    //        else
    //        {
    //            // Log the error.
    //            Log.Error(TAG, "Invalide");
    //        }
    //    }

    //    string GetGeofenceTransitionDetails(Context context, int geofenceTransition, IList<IGeofence> triggeringGeofences)
    //    {
    //        string geofenceTransitionString = GetTransitionString(geofenceTransition);

    //        var triggeringGeofencesIdsList = new List<string>();
    //        foreach (IGeofence geofence in triggeringGeofences)
    //        {
    //            triggeringGeofencesIdsList.Add(geofence.RequestId);
    //        }
    //        var triggeringGeofencesIdsString = string.Join(", ", triggeringGeofencesIdsList);

    //        return geofenceTransitionString + ": " + triggeringGeofencesIdsString;
    //    }

    //    void SendNotification(string notificationDetails)
    //    {
    //        var notificationIntent = new Intent(ApplicationContext, typeof(MainActivity));

    //        var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
    //        stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
    //        stackBuilder.AddNextIntent(notificationIntent);

    //        var notificationPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

    //        var builder = new NotificationCompat.Builder(this);
    //        builder.SetSmallIcon(Resource.Drawable.ic_notify)
    //            .SetColor(Color.Red)
    //            .SetContentTitle(notificationDetails)
    //            .SetContentText("Pouet")
    //            .SetContentIntent(notificationPendingIntent);

    //        builder.SetAutoCancel(true);

    //        var mNotificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
    //        mNotificationManager.Notify(0, builder.Build());
    //    }

    //    string GetTransitionString(int transitionType)
    //    {
    //        switch (transitionType)
    //        {
    //            case Geofence.GeofenceTransitionEnter:
    //                return "Entré ";
    //            case Geofence.GeofenceTransitionExit:
    //                return "Sortie ";
    //            default:
    //                return "Un truc ";
    //        }
    //    }
    //}


    [BroadcastReceiver(Name = "com.hemcefor.homeautomationapp.locationbroadcastreceiver", Enabled = true, Exported = true)]
    //[IntentFilter(new[] { TelephonyManager.ActionPhoneStateChanged })]
    //[IntentFilter(new[] { TelephonyManager.ActionRespondViaMessage })]
    public class LocationBroadcastReceiver : BroadcastReceiver, IOnCompleteListener
    {
        public void OnComplete(Task task)
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "reception d'une notif", ToastLength.Short).Show();
        }
    }
}