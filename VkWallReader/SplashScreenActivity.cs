using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using System.Threading.Tasks;
using VKontakte;

namespace VkWallReader
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : AppCompatActivity
    {
        bool isResumed=false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            VKSdk.WakeUpSession(this,
            response =>
            {
                if (isResumed)
                {
                    if (response == VKSdk.LoginState.LoggedOut)
                    {
                        ShowLoginPage();
                    }
                    else if (response == VKSdk.LoginState.LoggedIn)
                    {
                        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    }
                }
            },
            error =>
            {
                 //TODO: log error
            });
        }      
        
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
            
            isResumed = true;
            if (!VKSdk.IsLoggedIn)
                ShowLoginPage();
        }

        async void SimulateStartup()
        {          
            await Task.Delay(1000);           
        }

        private void ShowLoginPage()
        {
            StartActivity(new Intent(Application.Context, typeof(LogInActivity)));
        }
    }
}