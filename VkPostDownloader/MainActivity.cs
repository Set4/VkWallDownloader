using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using VKontakte;
using SQLite;
using System.IO;
using VkPostDownloader.UtilityClasses;

namespace VkPostDownloader
{
    [Activity(Theme = "@style/MyTheme.Base", Label = "VkPostDownloader", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        bool isResumed = false;      
        string dataBasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "appDb.db3");

        public SQLiteAsyncConnection Connection { get; private set; }
      
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.container_layout);         
          
            VKSdk.WakeUpSession(this,
             response =>
             {
                 if (isResumed)
                 {
                     if (response == VKSdk.LoginState.LoggedOut)
                     {
                         ShowLogin();
                     }
                     else if (response == VKSdk.LoginState.LoggedIn)
                     {
                         FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                         fragmentTx.Replace(Resource.Id.fragment_container, new FollowersFragment()).Commit();
                     }
                 }
             },
             error =>
             {
                 //TODO: log error
                 Console.WriteLine("WakeUpSession error: " + error);
             });


            Connection=await DbHelper.CreateConnection(dataBasePath);
        }

        protected override void OnResume()
        {
            base.OnResume();
            isResumed = true;
            if (!VKSdk.IsLoggedIn)
                ShowLogin();
        }

        protected override void OnPause()
        {
            isResumed = false;
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            DbHelper.ResetPoolConnection();
            base.OnDestroy();
        }

        private void ShowLogin()
        {
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            fragmentTx.Replace(Resource.Id.fragment_container, new LoginFragment()).CommitAllowingStateLoss();
        }
    }

    
}

