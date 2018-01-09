using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using VKontakte;

namespace VkWallReader
{
    [Application]
    public class MainApplication : Application
    {
        public static string[] MyScopes = 
        {
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

            tokenTracker.StartTracking();
            VKSdk.Initialize(this).WithPayments();
        }

        private class TokenTracker : VKAccessTokenTracker
        {
            public override void OnVKAccessTokenChanged(VKAccessToken oldToken, VKAccessToken newToken)
            {
                if (newToken == null)
                {
                    Intent intent = new Intent(Application.Context, typeof(MainActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
                    Application.Context.StartActivity(intent);
                }
              
            }
        }

      
    }
}