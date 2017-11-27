using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using VKontakte;
using Android.Content;
using SQLite;
using System.IO;
using System.Threading.Tasks;

namespace VkPostDownloader
{
    [Activity(Theme = "@style/MyTheme.Base", Label = "VkPostDownloader", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {

        public SQLiteAsyncConnection Connection { get; private set; }
        public Intent DownloadIntent { get; private set; }


        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            DownloadIntent = new Intent(this, typeof(GroupWallDownlodaderIntentService));
            StartService(DownloadIntent);
          



            VKSdk.WakeUpSession(this,
             response =>
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

             },
             error =>
             {
                 //TODO: log error
                 Console.WriteLine("WakeUpSession error: " + error);
             });


            await CreateConnection();
        }

        protected override void OnResume()
        {
            base.OnResume();
           
            if (!VKSdk.IsLoggedIn)
                ShowLogin();
        }

        protected override void OnPause()
        {            
            base.OnPause();
        }

        protected override void OnDestroy()
        {           
            base.OnDestroy();
           // SQLite.SQLiteAsyncConnection.ResetPool();
        }

        private void ShowLogin()
        {
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            fragmentTx.Replace(Resource.Id.fragment_container, new LoginFragment()).Commit();
        }


        private async Task CreateConnection()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "appDb.db3");
            try
            {
                if (!File.Exists(path))
                {
                    Connection = new SQLiteAsyncConnection(path);
                    await Connection.CreateTableAsync<GroupItem>();
                    await Connection.CreateTableAsync<PostItem>();
                    await Connection.CreateTableAsync<ImageItem>();
                }
                else
                    Connection = new SQLiteAsyncConnection(path);

               
            }
            catch (Exception ex)
            {
                //TODO: log error
                Console.WriteLine("CreateConnection error: " + ex);
            }
            return;
        }
    }

    
}

