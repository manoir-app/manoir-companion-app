using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomationApp.Droid
{
    public static class GeoFencingHelper
    {
        private static List<IGeofence> mGeofenceList = new List<IGeofence>();


        internal static async Task RefreshList()
        {
            Context ctx = Xamarin.Forms.Forms.Context;
            await RefreshList(ctx);
        }

        internal static async Task  RefreshList(Context ctx)
        {
            List<Home.Common.Model.Location> locs = null;
            try
            {
                locs = MainApiHelper.Get<List<Home.Common.Model.Location>>("/v1.0/locations");
                var svc = LocationServices.GetGeofencingClient(ctx);





                var intent = new Intent(ctx, typeof(LocationBroadcastReceiver));
                var src = PendingIntent.GetService(ctx, 0, intent, PendingIntentFlags.UpdateCurrent);

                mGeofenceList.Clear();
                foreach (var loc in locs)
                {
                    mGeofenceList.Add(new GeofenceBuilder()
                    .SetRequestId(loc.Id)
                    .SetCircularRegion(
                        (double)loc.Coordinates.Latitude,
                        (double)loc.Coordinates.Longitude,
                        50
                    )
                    .SetExpirationDuration(Geofence.NeverExpire)
                    .SetTransitionTypes(Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit)
                    .Build());
                }

                GeofencingRequest.Builder builder = new GeofencingRequest.Builder();
                builder.SetInitialTrigger(GeofencingRequest.InitialTriggerEnter);
                builder.AddGeofences(mGeofenceList);
                var bld = builder.Build();

                svc.AddGeofences(bld, src).AddOnCompleteListener(new LocationBroadcastReceiver());
            }
            catch(Exception ex)
            {

            }
        }
    }
}