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
using Android.Util;
using Android.Provider;
using Android.Net;
using static Android.Provider.ContactsContract;
using static Android.Provider.Contacts;

namespace MissedCallText
{
    [BroadcastReceiver(Enabled = true, Label = "SMS Receiver")]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]

    public class IncomingSms : BroadcastReceiver
    {//This class forwards the Recieves and forwards the SMS messages received on the device. 
        private const string Tag = "SMSBroadcastReceiver";

        public override void OnReceive(Context context, Intent intent)
        {//Run this method when an intent is received
            try
            {
                var prefs = context.GetSharedPreferences("ForwardNumber", 0);//get all preferences
                
                Log.Info(Tag, "Intent received: " + intent.Action);

                //if the recieved intent wasn't an SMS message then cancel
                if (intent.Action != "android.provider.Telephony.SMS_RECEIVED")
                {
                    return;
                }

                SmsMessage[] messages = Android.Provider.Telephony.Sms.Intents.GetMessagesFromIntent(intent);//messages recieved
                bool isNum = false;
                var sb = new StringBuilder();//string to put into body of forwarded message
                bool fromForwardNum = false;//Check if the text came from the existing forward number
                string beginning = "";//check what the message from the forward number starts with

                for (var i = 0; i < messages.Length; i++)
                {//check every message
                    if (messages[i].OriginatingAddress.Equals(prefs.GetString("Number", String.Empty)))
                    {//if it is coming from the forward number

                        if (messages[i].MessageBody.Length > 10)
                        {
                            beginning = messages[i].MessageBody.Substring(0, 10);
                        }
                        else
                        {
                            beginning = "short";
                            sb.Append(string.Format("{0}{1}", GetContact(context, messages[i].OriginatingAddress), System.Environment.NewLine));
                            sb.Append(messages[i].MessageBody.ToString());
                            fromForwardNum = true;
                        }
                        
                        isNum = Int64.TryParse(beginning, out long result);
                        if (isNum)
                        {//build string and set condition to forward to desired target
                            fromForwardNum = true;
                            //sb.Append(string.Format("{0}{1}",GetContact(context, messages[i].OriginatingAddress), System.Environment.NewLine));
                            sb.Append(messages[i].MessageBody.Substring(10));
                        }
                    }
                    else
                    {//build string for forward to user number
                        sb.Append(string.Format("SMS From: {0}{2}{1}{2}Body: {2}{3}", 
                            GetContact(context, messages[i].OriginatingAddress),
                            messages[i].OriginatingAddress, 
                            System.Environment.NewLine, 
                            messages[i].MessageBody));
                    }
                }
                if (sb != null && !fromForwardNum)
                {//forward to user number
                    SmsManager.Default.SendTextMessage(prefs.GetString("Number", string.Empty), null, sb.ToString(), null, null);
                }
                else if (sb != null && fromForwardNum && isNum)
                {//forward from user to different phone
                    SmsManager.Default.SendTextMessage(beginning, null, sb.ToString(), null, null);
                }
                else if (sb != null && fromForwardNum && !isNum)
                {
                    SmsManager.Default.SendTextMessage(prefs.GetString("Number", string.Empty), null, sb.ToString() + "SMS cannot be empty", null, null);
                }
                else
                {
                    SmsManager.Default.SendTextMessage(prefs.GetString("Number", string.Empty), null, "Error receiving message", null, null);
                }
            }
            catch(System.Exception e)
            {
                e.ToString();
            }
        }

        public string GetContact(Context context, string number)
        {//Get the name of the contact in the current phone and send with message
            ContentResolver resolver = context.ContentResolver;

            //Filter to limit the data returned for speed purposes
            string[] projection = { ContactsContract.Contacts.InterfaceConsts.DisplayName };

            Android.Net.Uri uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
            ICursor cursor = resolver.Query(uri, projection, ContactsContract.CommonDataKinds.Phone.NormalizedNumber + " LIKE '+1" + number + "'", null, null);
            cursor.MoveToLast();
            String name = "";
            if (cursor.Count > 0)
            {
                name = cursor.GetString(0);
            }
            return name;
        }
    }
}