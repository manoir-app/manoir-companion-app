using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeAutomationApp.Droid
{
    [Service()]
    public class AndroidAutoMessagingService : IntentService
    {
        public AndroidAutoMessagingService() : base("MessagingService")
        {

        }

        protected override void OnHandleIntent(Intent intent)
        {

        }
    }
}