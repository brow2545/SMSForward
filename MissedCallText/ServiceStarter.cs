using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;

namespace MissedCallText
{
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    [IntentFilter(new string[] { Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted, "android.intent.action.QUICKBOOT_POWERON", "com.htc.intent.action.QUICKBOOT_POWERON" })]

    public class ServiceStarter : BroadcastReceiver
    {//This class is used to start the service on boot
        readonly string DEFAULT_NUMBER = "0000000000";
        public override void OnReceive(Context context, Intent intent)
        {

            var prefs = context.GetSharedPreferences("ForwardNumber", 0);
            if (prefs.GetString("Number", string.Empty) != DEFAULT_NUMBER && prefs.GetString("Number", string.Empty).Length == 10)
            {
                try
                {
                    if (intent.Action.Equals(Intent.ActionBootCompleted))
                    {

                        Toast.MakeText(context, "Received start service intent!", ToastLength.Long).Show();
                        Intent serviceIntent = new Intent(context, typeof(ForwardService));
                        serviceIntent.AddFlags(ActivityFlags.NewTask);
                        context.StartActivity(serviceIntent);


                    }
                }
                catch (Exception ex)
                {
                    SmsManager.Default.SendTextMessage(prefs.GetString("Number", string.Empty), null, ex.ToString(), null, null);
                    throw ex;
                }
            }
        }
    }
}