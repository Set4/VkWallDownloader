using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace VkPostDownloader
{
    [Service]
    public class GroupWallDownlodaderIntentService : IntentService
    {
        int i = 0;
        public GroupWallDownlodaderIntentService() : base("DemoIntentService")
        {
        }

        protected override void OnHandleIntent(Intent intent)
        {
            int tm = intent.GetIntExtra("id", 0);

            Console.WriteLine("perform some long running work i={0}", tm); 
        }
    }
}