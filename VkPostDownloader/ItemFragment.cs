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
using static Android.Support.V4.Widget.SwipeRefreshLayout;
using System.Threading.Tasks;

namespace VkPostDownloader
{
    public class ItemFragment : Fragment, IOnRefreshListener
    {

        [InjectView(Resource.Id.toolbar_item)]
        Android.Support.V7.Widget.Toolbar toolbar;
        [InjectView(Resource.Id.rcView_itemPosts)]
        RecyclerView recyclerView;
        [InjectView(Resource.Id.swpRefresh_updateWalls)]
        Android.Support.V4.Widget.SwipeRefreshLayout swipeRefreshLayout;

        WallsItemAdapter adapter;

        static int currentGroupItemKey = 0;
        static int page = 0;
        static int countLoadItems = 10;
        int countMax = 0;

        string query = "select * from PostItem where GroupItemKey = ? LIMIT ? OFFSET ?";
        object[] queryArgs = new object[] { currentGroupItemKey,  countLoadItems,page* countLoadItems};

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Cheeseknife.Reset(this);


            swipeRefreshLayout.SetOnRefreshListener(this);
            // делаем повеселее
            // swipeRefreshLayout.setColorScheme(Resource.Color.b.color.blue, R.color.green, R.color.yellow, R.color.red);

            Bundle bundle = this.Arguments;
            currentGroupItemKey = bundle.GetInt("key");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.item_layout, null);
            Cheeseknife.Inject(this, view);

            ((AppCompatActivity)this.Activity).SetSupportActionBar(toolbar);
            ((AppCompatActivity)this.Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (recyclerView != null)
            {
                recyclerView.HasFixedSize = true;

                adapter = new WallsItemAdapter(((AppCompatActivity)this.Activity));

                var layoutManager = new LinearLayoutManager(this.Activity.BaseContext);
                var onScrollListener = new EndlessLoadOnScrollListener(layoutManager, countMax);
                onScrollListener.LoadMoreEvent += OnScrollListener_LoadMoreEvent;

                recyclerView.AddOnScrollListener(onScrollListener);
                recyclerView.SetLayoutManager(layoutManager);
                recyclerView.SetAdapter(adapter);
            }

            RegisterForContextMenu(toolbar);

            return view;
        }

        public async override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            await AddSearchResult(countLoadItems, page * countLoadItems);
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
            inflater.Inflate(Resource.Menu.action_menu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }




        private async void OnScrollListener_LoadMoreEvent(object sender, EventArgs e)
        {
            await AddSearchResult(countLoadItems, page * countLoadItems);
        }

        private async Task AddSearchResult(int count, int offset)
        {
            try
            {
                //TODO: progress bar
                countMax = await DbHelper.GetCountWalls(currentGroupItemKey, ((MainActivity)this.Activity).Connection);
                IEnumerable<PostItem> items = await DbHelper.GetItems<PostItem>(query: query, args: queryArgs, connection: ((MainActivity)this.Activity).Connection);
                adapter.AddRange(items.ToList());
                page++;
                //TODO:Progress bar
            }
            catch(Exception ex)
            {
                //TODO:Log
            }
            return;
        }



        public async void OnRefresh()
        {
            // говорим о том, что собираемся начать
           // Toast.makeText(this, R.string.refresh_started, Toast.LENGTH_SHORT).show();
            // начинаем показывать прогресс
            swipeRefreshLayout.Refreshing=true;
            // ждем 3 секунды и прячем прогресс
            page = 0;
            adapter.Clear();
            await AddSearchResult(countLoadItems, page * countLoadItems);
            swipeRefreshLayout.Refreshing = false;
        }
    }
}