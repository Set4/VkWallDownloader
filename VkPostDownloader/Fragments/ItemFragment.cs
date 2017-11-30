﻿using System;
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
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using static Android.Support.V4.Widget.SwipeRefreshLayout;
using System.Threading.Tasks;
using FFImageLoading;
using FFImageLoading.Views;
using VkPostDownloader.UtilityClasses;
using VkPostDownloader.Model;

namespace VkPostDownloader
{
    public class ItemFragment : Fragment, IOnRefreshListener
    {
        Android.Support.V7.Widget.Toolbar toolbar;
        RecyclerView recyclerView;
        Android.Support.V4.Widget.SwipeRefreshLayout swipeRefreshLayout;     
        public ImageViewAsync image;

        WallsItemAdapter adapter;

        static int currentGroupItemKey = 0;
        static int page = 0;
        static int countLoadItems = 10;
        int countMax = 0;

        string query = "select * from PostItem where GroupItemKey = ? LIMIT ? OFFSET ?";
        object[] queryArgs = new object[] { currentGroupItemKey,  countLoadItems,page* countLoadItems};

        public override void OnCreate(Bundle savedInstanceState)
        {
            Bundle bundle = this.Arguments;
            currentGroupItemKey = bundle.GetInt("key");

            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.item_layout, null);

            toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_item);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.rcView_itemPosts);
            swipeRefreshLayout = view.FindViewById<Android.Support.V4.Widget.SwipeRefreshLayout>(Resource.Id.swpRefresh_updateWalls);
            image = view.FindViewById<ImageViewAsync>(Resource.Id.imgView_imageItem);

            swipeRefreshLayout.SetOnRefreshListener(this);

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

            var item = await GetGroupItem(currentGroupItemKey);
            if (item != null)
            {               
                ((AppCompatActivity)this.Activity).Title = item.Name;
            
                await ImageService.Instance.LoadFile(item.PhotoPath)
                 .Retry(2, 200)
                 .LoadingPlaceholder("ic_done", FFImageLoading.Work.ImageSource.CompiledResource)
                 .ErrorPlaceholder("ic_action", FFImageLoading.Work.ImageSource.CompiledResource)
                 .Error(e => System.Diagnostics.Debug.WriteLine(e.Message)) //TODO: log                                 
                 .IntoAsync(image);
            }
            await GetPOsts(countLoadItems, page * countLoadItems);
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
                case Android.Resource.Id.Home:
                    // onBackPressed();
                    break;
                case Resource.Id.action_settingsSearch:
                    // onBackPressed();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.action_menu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }
        
        public async void OnRefresh()
        {
            swipeRefreshLayout.Refreshing = true;
            page = 0;
            adapter.Clear();
            await GetPOsts(countLoadItems, page * countLoadItems);
            swipeRefreshLayout.Refreshing = false;
        }

        private async void OnScrollListener_LoadMoreEvent(object sender, EventArgs e)
        {
            await GetPOsts(countLoadItems, page * countLoadItems);
        }

        private async Task GetPOsts(int count, int offset)
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

        private async Task<GroupItem> GetGroupItem(int key)
        {
            GroupItem item = null;
            try
            {
                item= await DbHelper.GetItem(key, ((MainActivity)this.Activity).Connection);
            }
            catch (Exception ex)
            {
                 //TODO:Log
            }
            return item;
        }
    }
}