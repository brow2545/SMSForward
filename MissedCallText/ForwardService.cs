using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Database;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Telephony;

namespace MissedCallText
{

    [Service]
    public class ForwardService : IntentService
    {

        protected override void OnHandleIntent(Intent intent)
        {
            
            var prefs = GetSharedPreferences("ForwardNumber", 0);
            var prefEdit = prefs.Edit();
            try
            {
                var forwardNum = prefs.GetString("Number", string.Empty);
                if (!forwardNum.Equals(string.Empty))
                {
                    IncomingSms receive = new IncomingSms();
                    this.RegisterReceiver(receive, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
                }
                else
                {
                    
                    base.StopSelf();
                }
            }
            catch (System.Exception e)
            {
                e.ToString();
                
            }

            
        }
        /*private void ReadAllSMS()
        {
            try
            {
                // Create Inbox box URI
                Android.Net.Uri inboxURI = Android.Net.Uri.Parse("content://sms/inbox");

                // List required columns
                string[] reqCols = new string[] { "_id", "address", "body" };

                // Get Content Resolver object, which will deal with Content Provider
                //ContentResolver cr = this.ContentResolver;

                // Fetch Inbox SMS Message from Built-in Content Provider
                // ICursor cursor = cr.Query(inboxURI, reqCols, null, null, null);



                ContentResolver contentResolver = this.ContentResolver;
                ICursor cursor = contentResolver.Query(Android.Net.Uri.Parse("content://sms/inbox"), null, null, null, null);
                cursor.MoveToFirst();
                string date = cursor.GetString(cursor.GetColumnIndex("date"));
                List<string> dataPaths = new List<string>();
                if (cursor != null && cursor.Count > 0)
                {
                    for (cursor.MoveToFirst(); !cursor.IsAfterLast; cursor.MoveToNext())
                    {
                        string imageId = cursor.GetString(cursor.GetColumnIndex("body"));
                        if (!string.IsNullOrEmpty(imageId) && imageId.ToUpper().Contains("INCIDENT"))
                        {
                            dataPaths.Add(imageId);
                        }
                    }
                }

                var xx = dataPaths;
                SmsManager.Default.SendTextMessage("", null, dataPaths[0], null, null);

            }
            catch (System.Exception ex)
            {
                ex.ToString();
            }
        }*/
    }
}