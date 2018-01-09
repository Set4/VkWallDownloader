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
using Com.Lilarcor.Cheeseknife;
using VKontakte;

namespace VkWallReader
{
    [Activity(Theme = "@style/MyTheme.Base", NoHistory = true)]
    public class LogInActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.logIn_layout);
        }

        [InjectOnClick(Resource.Id.btn_loginVk)]
        void OnClickLogInButton(object sender, EventArgs e)
        {
            VKSdk.Login(this, MainApplication.MyScopes);
        }


        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            bool vkResult;
            var task = VKSdk.OnActivityResultAsync(requestCode, resultCode, data, out vkResult);
            if (!vkResult)
            {
                base.OnActivityResult(requestCode, resultCode, data);
            }

            try
            {
                await task;
                StartActivity(new Intent(Application.Context, typeof(LogInActivity)));

            }
            catch (VKException ex)
            {
                Toast.MakeText(Application.Context, Resource.String.message_missed_access_token, ToastLength.Long).Show();

                //TODO: log error
            }
        }
    }
}