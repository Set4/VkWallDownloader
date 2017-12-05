using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Lilarcor.Cheeseknife;
using BottomNavigationBar;
using BottomNavigationBar.Listeners;

namespace VkWallReader
{
    public class MainFragment : Fragment, IOnMenuTabClickListener
    {
        private BottomBar bottomBar;

       

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.main_layout, null);

            bottomBar = BottomBar.Attach(this.Activity, savedInstanceState);
            bottomBar.UseFixedMode();
            bottomBar.SetItems(Resource.Menu.bottombar_menu);
            bottomBar.SetOnMenuTabClickListener(this);

          
            bottomBar.UseDarkThemeWithAlpha();
            bottomBar.SetActiveTabColor(new Android.Graphics.Color(this.Activity.BaseContext.GetColor(Resource.Color.accent)));

            Cheeseknife.Inject(this, view);
            return view;
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            Cheeseknife.Reset(this);
        }

        public void OnMenuTabReSelected(int menuItemId)
        {
            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            fragmentTx.Replace(Resource.Id.page_container, new SearchFragment()).CommitAllowingStateLoss();
            //switch (menuItemId)
            //{

            //    case 0: 
            //        fragmentTx.Replace(Resource.Id.page_container, new FollowerFragment()).CommitAllowingStateLoss();
            //        break;

            //    case 1:
                  
            //        break;
            //    default: throw new Exception("neizvestnii punct menu");
            //}
            
           
          

            Toast.MakeText(this.Activity.BaseContext, "Re"+ menuItemId.ToString(), ToastLength.Short).Show();
        }

        public void OnMenuTabSelected(int menuItemId)
        {
            Toast.MakeText(this.Activity.BaseContext, menuItemId.ToString(), ToastLength.Short).Show();
        }
    }
}