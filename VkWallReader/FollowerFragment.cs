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
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;

namespace VkWallReader
{
    public class FollowerFragment : Fragment
    {
        [InjectView(Resource.Id.toolbar)]
        Toolbar toolbar;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.followers_layout, null);
            Cheeseknife.Inject(this, view);

            // var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            //((AppCompatActivity)this.Activity).SetActionBar(toolbar);
            ((AppCompatActivity)this.Activity).SetSupportActionBar(toolbar);
            this.Activity.Title="My Toolbar";
            
            return view;            
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            Cheeseknife.Reset(this);
        }


        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.menu_followers_toolbar, menu);

           /* var item = menu.FindItem(Resource.Id.action_search);
            var searchView = MenuItemCompat.GetActionView(item);

            _searchView = searchView.JavaCast<Android.Support.V7.Widget.SearchView>();
            _searchView.QueryTextChange += (s, e) => _adapter.Filter.InvokeFilter(e.NewText);
            _searchView.QueryTextSubmit += (s, e) =>
            {
                this.Activity.Window.SetSoftInputMode(SoftInput.StateHidden);
                e.Handled = true;
            };

            MenuItemCompat.SetOnActionExpandListener(item, new SearchViewExpandListener(_adapter));
            */
            base.OnCreateOptionsMenu(menu, inflater);
        }

     
        public override bool OnOptionsItemSelected(IMenuItem item)
        {           
            return base.OnOptionsItemSelected(item);
        }
    }
}