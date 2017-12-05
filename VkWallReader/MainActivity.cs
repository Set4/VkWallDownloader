using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Com.Lilarcor.Cheeseknife;

namespace VkWallReader
{
    [Activity(Theme = "@style/MyTheme.Base", Label = "VkWallReader", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {    
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);            
            SetContentView(Resource.Layout.Main);
            Cheeseknife.Inject(this);

            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            fragmentTx.Replace(Resource.Id.activity_fragment_container, new MainFragment()).CommitAllowingStateLoss();
        }
    }
}

