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
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using System.Threading.Tasks;
using VKontakte.API;
using Newtonsoft.Json;
using VKontakte;

namespace VkPostDownloader
{
    public class SearchGroupFragment : Fragment
    {
        [InjectView(Resource.Id.toolbar)]
        Android.Support.V7.Widget.Toolbar toolbar;
        [InjectView(Resource.Id.recyclerV_searchGroups)]
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
            Cheeseknife.Inject(this, view);

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
            Cheeseknife.Reset(this);
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
                case 16908332:
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

            this.searchView.QueryTextSubmit += _searchView_QueryTextSubmit;          
            this.searchView.SetIconifiedByDefault(false);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        private async void _searchView_QueryTextSubmit(object sender, Android.Support.V7.Widget.SearchView.QueryTextSubmitEventArgs e)
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
            adapter.AddRange(await GetSearchResult(seachQuery, count, page * count));
            page++;
            //TODO:Progress bar

            return;
        }


        #region Search

        private Task<List<GroupItem>> GetSearchResult(string query, int count, int offset)
        {
            return Task.Run(async () =>
            {
                List<GroupItem> items = new List<GroupItem>();
                try
                {
                    var request = VKApi.Groups.Search(VKParameters.From(VKApiConst.Q, query, VKApiConst.Offset, offset, VKApiConst.Count, count, "type", "group, page", VKApiConst.AccessToken, VKSdk.AccessToken));
                    var response = await request.ExecuteAsync();

                    var data = JsonConvert.DeserializeObject<Rootobject>(response.Json.ToString());
                    //var jsonArray = response.Json.OptJSONArray(@"response");

                    GroupItem itemGroup;

                    if (data.response.count > 0)
                    {
                        foreach (var item in RemoveCloseGroups(data.response.items))
                        {
                            itemGroup = ConvertToGroupItem(item);
                            itemGroup.IsExist = await DbHelper.CheckIsExist<GroupItem>(itemGroup.Id, ((MainActivity)this.Activity).Connection);
                            items.Add(itemGroup);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ToDO: log
                }

                return items;
            });
        }

        private IEnumerable<GroupJsonModel> RemoveCloseGroups(IEnumerable<GroupJsonModel> items)
        {
            return items.Where(i => i.is_closed == 0 || i.is_admin == 1 || i.is_member == 1);
        }

        private GroupItem ConvertToGroupItem(GroupJsonModel item)
        {
            return new GroupItem()
            {
                Id = item.id,
                Name = item.name,
                Photo = item.photo_100,
                ScreenName = item.screen_name,
                Type = item.type
            };
        }

        public class Rootobject
        {
            public Response response { get; set; }
        }

        public class Response
        {
            public int count { get; set; }
            public GroupJsonModel[] items { get; set; }
        }

        public class GroupJsonModel
        {
            public int id { get; set; }
            public string name { get; set; }
            public string screen_name { get; set; }
            public int is_closed { get; set; }
            public string type { get; set; }
            public int is_admin { get; set; }
            public int is_member { get; set; }
            public string photo_50 { get; set; }
            public string photo_100 { get; set; }
            public string photo_200 { get; set; }
        }


       

        #endregion


    }
}