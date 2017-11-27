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
using VKontakte;
using VKontakte.Utils;

namespace VkPostDownloader
{
    [Application]
    public class MainApplication : Application
    {

        public static string[] MyScopes = {
           VKScope.Groups,
           VKScope.Wall,
           VKScope.Photos,
           VKScope.Offline
        };

        private readonly TokenTracker tokenTracker = new TokenTracker();

        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            // list the app fingerprints
            string[] fingerprints = VKUtil.GetCertificateFingerprint(this, PackageName);
            foreach (var fingerprint in fingerprints)
            {
                Console.WriteLine("Detected Fingerprint: " + fingerprint);
            }

            // setup VK
            tokenTracker.StartTracking();
            VKSdk.Initialize(this).WithPayments();
        }

        private class TokenTracker : VKAccessTokenTracker
        {
            public override void OnVKAccessTokenChanged(VKAccessToken oldToken, VKAccessToken newToken)
            {
                if (newToken == null)
                {
                    Toast.MakeText(Application.Context, Resource.String.missed_access_token_message, ToastLength.Long).Show();
                    Intent intent = new Intent(Application.Context, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
                    Application.Context.StartActivity(intent);
                }
              
            }
        }

      
    }
}