using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Telephony;
using Android.Content;
using Android.Util;
using Java.Lang;
using Android.Database;
using System.Collections.Generic;

namespace MissedCallText
{
    
    [Activity(Label = "MissedCallText", MainLauncher = true)]
    public class MainActivity : Activity
    {
        //static readonly string SERVICE_STARTED_KEY = "has_service_been_started";

        //bool isStarted = false;
        bool clearNumText = false;
        readonly string DEFAULT_NUMBER = "0000000000";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //When program starts this method runs

            var prefs = GetSharedPreferences("ForwardNumber", 0);//Phone number stored if it exists
            var prefEdit = prefs.Edit();

            SetContentView(Resource.Layout.Main);// Set our view from the "main" layout resource  

            //Reference all the buttons and text boxes used on the app screen
            var sendSMS = FindViewById<Button>(Resource.Id.sendSMS);
            var setForwardNum = FindViewById<Button>(Resource.Id.setForward);
            var forwardNum = FindViewById<TextView>(Resource.Id.forwardNumber);
            var stopService = FindViewById<Button>(Resource.Id.stopService);
            var startService = FindViewById<Button>(Resource.Id.startService);

            //Create intent for the service to run in background from app
            Intent serviceIntent = new Intent(base.BaseContext, typeof(ForwardService));

            //stop any service that may have started before
            StopService(serviceIntent);
            stopService.Enabled = false;

            base.OnCreate(savedInstanceState);
            

            try
            {//Gets the forward number and puts it into the text box if it exists
                forwardNum.Text = prefs.GetString("Number", string.Empty);
            }
            catch (System.Exception e)
            {//if it doesn't exist then it puts a placeholder text
                e.ToString();
                forwardNum.Text = DEFAULT_NUMBER;
            }

            sendSMS.Click += (sender, e) => 
            {//Test message to verify that the program works
                if(!prefs.GetString("Number", string.Empty).Equals(DEFAULT_NUMBER) && prefs.GetString("Number", string.Empty).Length == 10)
                {
                    SmsManager.Default.SendTextMessage(forwardNum.Text, null, "Test Succeeded", null, null);
                }
                
            };
            
            setForwardNum.Click += (sender, e) => 
            {//sets the forward number entered by the user
                if (forwardNum.Text.Length == 10 && !forwardNum.Text.Equals(DEFAULT_NUMBER) && !clearNumText)
                {
                    prefEdit.PutString("Number", forwardNum.Text);
                    prefEdit.Commit();
                    setForwardNum.Text = "CLEAR FORWARD NUMBER";
                    clearNumText = true;
                }
                else if (clearNumText)
                {
                    setForwardNum.Text = "SET FORWARD NUMBER";
                    prefEdit.PutString("Number", DEFAULT_NUMBER);
                    prefEdit.Commit();
                    clearNumText = false;
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Number entered is invalid", ToastLength.Long).Show();
                }
                
            };
            


            startService.Click += (sender, e) =>
            {//manually starts the forwarding service
                serviceIntent.AddFlags(ActivityFlags.NewTask);
                StartService(serviceIntent);
                stopService.Enabled = true;
                startService.Enabled = false;
            };

            stopService.Click += (sender, e) =>
            {//manually stop the service
                StopService(serviceIntent);
                base.OnDestroy();
                
                startService.Enabled = true;
                stopService.Enabled = false;
                
            };
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

        /*protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(SERVICE_STARTED_KEY, isStarted);
            base.OnSaveInstanceState(outState);
        }*/

    }

    

}

