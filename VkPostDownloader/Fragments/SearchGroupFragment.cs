using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using System.Threading.Tasks;
using VkPostDownloader.UtilityClasses;

namespace VkPostDownloader
{
    public class SearchGroupFragment : Fragment
    {       
        Android.Support.V7.Widget.Toolbar toolbar;
        RecyclerView recyclerView;

        SearchGroupsItemAdapter adapter;

        string seachQuery = String.Empty;
        readonly int count = 20;
        int page = 0;
        static readonly int countMax = 1000;

        private Android.Support.V7.Widget.SearchView searchView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetHasOptionsMenu(true);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.search_layout, null);

            toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerV_searchGroups);

            ((AppCompatActivity)this.Activity).SetSupportActionBar(toolbar);
            ((AppCompatActivity)this.Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);


            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;

                adapter = new SearchGroupsItemAdapter(((AppCompatActivity)this.Activity));

                var layoutManager = new LinearLayoutManager(this.Activity.BaseContext);
                var onScrollListener = new EndlessLoadOnScrollListener(layoutManager, countMax);
                onScrollListener.LoadMoreEvent += OnScrollListener_LoadMoreEvent; 

                recyclerView.AddOnScrollListener(onScrollListener);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);
            }

            return view;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.home:
                    // onBackPressed();
                    break;
                case Resource.Id.homeAsUp:
                    //  onBackPressed();
                    break;
                case Android.Resource.Id.Icon:
                    // onBackPressed();
                    break;
                case Resource.Id.action_settingsSearch:

                    break;

            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.search_menu, menu);

            var item = menu.FindItem(Resource.Id.action_searchNet);

            var searchView = MenuItemCompat.GetActionView(item);
            this.searchView = searchView.JavaCast<Android.Support.V7.Widget.SearchView>();

            this.searchView.QueryTextSubmit += SearchViewQueryTextSubmit;          
            this.searchView.SetIconifiedByDefault(false);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        private async void SearchViewQueryTextSubmit(object sender, Android.Support.V7.Widget.SearchView.QueryTextSubmitEventArgs e)
        {
            seachQuery = e.Query;
            e.Handled = true;
            adapter.Clear();
            await AddSearchResult(seachQuery, count, page * count);
        }

        private async void OnScrollListener_LoadMoreEvent(object sender, EventArgs e)
        {
            await AddSearchResult(seachQuery, count, page * count);
        }

        private async Task AddSearchResult(string query, int count, int offset)
        {
            //TODO: progress bar
            adapter.AddRange(await GroupsSearchHelper.GetSearchResult(seachQuery, count, page * count, ((MainActivity)this.Activity).Connection));
            page++;
            //TODO:Progress bar

            return;
        }
    }
}