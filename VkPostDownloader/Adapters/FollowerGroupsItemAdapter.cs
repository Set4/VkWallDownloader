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
using Android.Support.V7.Widget;
using Com.Lilarcor.Cheeseknife;
using Java.Lang;
using Android.Support.V4.View;
using FFImageLoading.Views;
using FFImageLoading;
using VkPostDownloader.Model;
using VkPostDownloader.UtilityClasses.CommonClasses;

namespace VkPostDownloader.Adapters
{
    class FollowerGroupsItemAdapter : RecyclerView.Adapter, IFilterable
    {
        private List<GroupItem> _items;
        private List<GroupItem> _originalData;
        private Activity _context;

        public Filter Filter { get; private set; }

        public event EventHandler<int> ItemClick;

        
        public FollowerGroupsItemAdapter(IEnumerable<GroupItem> items, Activity context)
        {
            _items = items.OrderBy(s => s.Name).ToList();
            _context = context;
            Filter = new FollowerGroupItemFilter(this);
        }

        public void AddRange(List<GroupItem> items)
        {
            items.AddRange(items);
            this.NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.vh_groupltem, parent, false);
            FollowerGroupViewHolder vh = new FollowerGroupViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            FollowerGroupViewHolder vh = holder as FollowerGroupViewHolder;
        
            ImageService.Instance.LoadFile(FFImageLoading.Work.ImageSource.ApplicationBundle+_items[position].Photo)
              .Retry(2, 200)
              .LoadingPlaceholder("ic_done", FFImageLoading.Work.ImageSource.CompiledResource)
              .ErrorPlaceholder("ic_action", FFImageLoading.Work.ImageSource.CompiledResource)
              .Error(e => System.Diagnostics.Debug.WriteLine(e.Message)) //TODO: log                                 
              .IntoAsync(vh.Image);

            vh.Name.Text = _items[position].Name;
            vh.CountWall.Text = _items[position].CountPosts.ToString();            
            vh.KeyItem = _items[position].Key;
        }

        private void OnClick(int position)
        {
            Bundle bundle = new Bundle();
            bundle.PutInt("key", _items[position].Key);

            FragmentTransaction fragmentTx = _context.FragmentManager.BeginTransaction();
            Fragment fagment = new ItemFragment();
            fagment.Arguments = bundle;
            fragmentTx.Replace(Resource.Id.fragment_container, fagment).AddToBackStack(null).Commit();
        }

        public override int ItemCount
        {
            get { return _items.Count; }
        }

        class FollowerGroupViewHolder : RecyclerView.ViewHolder, Android.Support.V7.Widget.PopupMenu.IOnMenuItemClickListener, IMenuItemOnMenuItemClickListener
        {
            [InjectView(Resource.Id.txtView_nameGroup)]
            public TextView Name { get; private set; }
            [InjectView(Resource.Id.txtView_countGroup)]
            public TextView CountWall { get; private set; }
            [InjectView(Resource.Id.imgView_imageGroup)]
            public ImageViewAsync Image { get; private set; }
            [InjectView(Resource.Id.btn_popupMenu)]
            public ImageButton Button { get; private set; }

            public int KeyItem { get; set; }

            public FollowerGroupViewHolder(View view, Action<int> listener) : base(view)
            {
                Cheeseknife.Inject(this, view);

                Button.Click += (s, e) =>
                    {
                        Android.Support.V7.Widget.PopupMenu popup = new Android.Support.V7.Widget.PopupMenu(view.Context, (View)s);
                        popup.Inflate(Resource.Menu.popup_menu);

                        popup.SetOnMenuItemClickListener(this);
                        popup.Show();
                    };

                view.Click += (sender, e) => listener(base.Position);
            }

          

            public bool OnMenuItemClick(IMenuItem item)
            {
                switch (item.ItemId)
                {
                    case Resource.Id.popup_item_refresh:
                    //  DbHelper.ClearPosts(Button.Tag)
                        break;

                    case Resource.Id.popup_item_delete:
                        //Do stuff

                        break;
                }
                return true;
            }         
        }

        class FollowerGroupItemFilter : Filter
        {
            private readonly FollowerGroupsItemAdapter _adapter;
            public FollowerGroupItemFilter(FollowerGroupsItemAdapter adapter)
            {
                _adapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObj = new FilterResults();
                var results = new List<GroupItem>();
                if (_adapter._originalData == null)
                    _adapter._originalData = _adapter._items;

                if (constraint == null) return returnObj;

                if (_adapter._originalData != null && _adapter._originalData.Any())
                {
                    results.AddRange(
                        _adapter._originalData.Where(
                            chemical => chemical.Name.ToLower().Contains(constraint.ToString())));
                }
                
                returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
                returnObj.Count = results.Count;

                constraint.Dispose();

                return returnObj;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                using (var values = results.Values)
                    _adapter._items = values.ToArray<Java.Lang.Object>()
                        .Select(r => r.ToNetObject<GroupItem>()).ToList();

                _adapter.NotifyDataSetChanged();

                constraint.Dispose();
                results.Dispose();
            }
        }
    }
}