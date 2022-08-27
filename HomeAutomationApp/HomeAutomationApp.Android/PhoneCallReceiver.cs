using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Telephony;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomationApp.Droid
{
    [BroadcastReceiver(Name = "com.hemcefor.homeautomationapp.phonereceiver", Enabled = true, Exported = true)]
    [IntentFilter(new[] { TelephonyManager.ActionPhoneStateChanged })]
    [IntentFilter(new[] { TelephonyManager.ActionRespondViaMessage })]

    public class PhoneBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {

            //Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();

            string action = intent.Action;
            if (action.Equals(TelephonyManager.ActionPhoneStateChanged))
            {
                try
                {
                    HandlePhoneStateChange(context, intent, action);
                }
                catch(Exception ex)
                {
                    Log.Error("phone", ex.ToString());
                }
            }
        }

        private void HandlePhoneStateChange(Context context, Intent intent, string action)
        {
            string state = intent.HasExtra(TelephonyManager.ExtraState) ? intent.GetStringExtra(TelephonyManager.ExtraState) : "";
            string number = intent.HasExtra(TelephonyManager.ExtraIncomingNumber) ? intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber) : "";

            if(state != null && state.Equals(TelephonyManager.ExtraStateOffhook, StringComparison.CurrentCultureIgnoreCase))
            {

            }
        }
    }
}