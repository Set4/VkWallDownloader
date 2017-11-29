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
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.View;
using System.Threading.Tasks;
using VkPostDownloader.Adapters;
using VkPostDownloader.UtilityClasses;
using VkPostDownloader.Model;

namespace VkPostDownloader
{
    public class FollowersFragment : Fragment
    {
        [InjectView(Resource.Id.drawer_layout)]
        DrawerLayout drawerLayout;
        [InjectView(Resource.Id.nav_view)]
        NavigationView navigationView;
        [InjectView(Resource.Id.toolbar)]
        Android.Support.V7.Widget.Toolbar toolbar;
        [InjectView(Resource.Id.recyclerV_followergroups)]
        RecyclerView recyclerView;
        [InjectView(Resource.Id.prBar_groupsLoacalLoading)]
        ProgressBar progressBar;


        private Android.Support.V7.Widget.SearchView _searchView;
        private FollowerGroupsItemAdapter _adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetHasOptionsMenu(true);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.followers_layout, null);
            Cheeseknife.Inject(this, view);

            ((AppCompatActivity)this.Activity).SetSupportActionBar(toolbar);
            var drawerToggle = new ActionBarDrawerToggle(this.Activity, drawerLayout, toolbar, Resource.String.drawer_open, Resource.String.drawer_close);
            drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            setupDrawerContent(navigationView);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            return view;
        }

        public async override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;

                _adapter = new FollowerGroupsItemAdapter(await GetFollowerGroups(), this.Activity);

                var layoutManager = new LinearLayoutManager(this.Activity.BaseContext);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(_adapter);
            }
        }
             
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            Cheeseknife.Reset(this);
        }

        private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            var menuItem = e.MenuItem;
            menuItem.SetChecked(!menuItem.IsChecked);

            Fragment fragment = null;

            switch (menuItem.ItemId)
            {
                case Resource.Id.nav_followers:
                    fragment = new FollowersFragment();
                    break;
                case Resource.Id.nav_search:
                    fragment = new SearchGroupFragment();
                    break;
                case Resource.Id.nav_logout:
                    fragment = new LogoutFragment();
                    break;
                default: throw new Exception("неизвестный пункт меню");
            }

            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            fragmentTx.Replace(Resource.Id.fragment_container, fragment).AddToBackStack(null).Commit();

            drawerLayout.CloseDrawers();
        }

        private void setupDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);
                drawerLayout.CloseDrawers();
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_add:

                    FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                    fragmentTx.Replace(Resource.Id.fragment_container, new SearchGroupFragment()).AddToBackStack(null).Commit();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.action_menu, menu);

            var item = menu.FindItem(Resource.Id.action_search);
            var searchView = MenuItemCompat.GetActionView(item);

            _searchView = searchView.JavaCast<Android.Support.V7.Widget.SearchView>();
            _searchView.QueryTextChange += (s, e) => _adapter.Filter.InvokeFilter(e.NewText);
            _searchView.QueryTextSubmit += (s, e) =>
            {                
                this.Activity.Window.SetSoftInputMode(SoftInput.StateHidden);
                e.Handled = true;
            };

            MenuItemCompat.SetOnActionExpandListener(item, new SearchViewExpandListener(_adapter));
            
            base.OnCreateOptionsMenu(menu, inflater);
        }

        private async Task<IEnumerable<GroupItem>> GetFollowerGroups()
        {
            IEnumerable<GroupItem> items = null;
            try
            {
                progressBar.Visibility = ViewStates.Visible;
                recyclerView.Visibility = ViewStates.Gone;

                items = await DbHelper.GetAllItems<GroupItem>(((MainActivity)this.Activity).Connection);
                foreach(var item in items)
                {
                    item.CountPosts= await DbHelper.GetCountWalls(item.Key,((MainActivity)this.Activity).Connection);
                }
            }
            catch (Exception ex)
            {
                //TODO: log error
                Console.WriteLine("GetFollowerGroups error: " + ex);
            }
            finally
            {
                progressBar.Visibility = ViewStates.Gone;
                recyclerView.Visibility = ViewStates.Visible;
            }
            return items;
        }
    }
}