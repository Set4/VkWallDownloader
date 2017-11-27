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
using Android.Support.V7.App;

namespace VkPostDownloader
{
    public class LogoutFragment : Fragment
    {
       
        [InjectView(Resource.Id.toolbar)]
        Android.Support.V7.Widget.Toolbar toolbar;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.logout_layout, container, false);

             Cheeseknife.Inject(this, view);

            ((AppCompatActivity)this.Activity).SetSupportActionBar(toolbar);
            ((AppCompatActivity)this.Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);


            return view;
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            Cheeseknife.Reset(this);
        }
        [InjectOnClick(Resource.Id.btn_logout)]
        void OnClickMyButton(object sender, EventArgs e)
        {
            VKSdk.Logout();
            if (!VKSdk.IsLoggedIn)
            {
                FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                fragmentTx.Replace(Resource.Id.fragment_container, new LoginFragment()).CommitAllowingStateLoss();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //switch (item.ItemId)
            //{
            //    case Resource.Id.home:
            //        this.Activity.OnBackPressed();
            //        break;
            //    case Resource.Id.homeAsUp:
            //        this.Activity.OnBackPressed();
            //        break;
            //    case 16908332:
            //        this.Activity.OnBackPressed();
            //        break;
            //    case Resource.Id.action_settingsSearch:

            //        break;

            //}
            return base.OnOptionsItemSelected(item);
        }
    }
}