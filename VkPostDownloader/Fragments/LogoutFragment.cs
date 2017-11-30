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
using Android.Support.V7.App;

namespace VkPostDownloader
{
    public class LogoutFragment : Fragment
    {
        Android.Support.V7.Widget.Toolbar toolbar;
        Button logout;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.logout_layout, container, false);

            toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            logout = view.FindViewById<Button>(Resource.Id.btn_logout);
            logout.Click += OnClickMyButton;

            ((AppCompatActivity)this.Activity).SetSupportActionBar(toolbar);
            ((AppCompatActivity)this.Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            return view;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }
        
        void OnClickMyButton(object sender, EventArgs e)
        {
            VKSdk.Logout();
            if (!VKSdk.IsLoggedIn)
            {
                FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                fragmentTx.Replace(Resource.Id.fragment_container, new LoginFragment()).CommitAllowingStateLoss();
            }
        }      
    }
}