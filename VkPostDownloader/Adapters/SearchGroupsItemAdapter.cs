using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Com.Lilarcor.Cheeseknife;
using FFImageLoading;
using FFImageLoading.Views;
using System.Threading.Tasks;
using VKontakte.API;
using VKontakte;
using Newtonsoft.Json;
using VkPostDownloader.UtilityClasses;
using VkPostDownloader.Model;
using VkPostDownloader.Model.Json.Wall;

namespace VkPostDownloader
{
    class SearchGroupsItemAdapter : RecyclerView.Adapter
    {
        private Activity _activity;
        public List<GroupItem> items;
        
        public SearchGroupsItemAdapter(Activity activity)
        {
            this._activity = activity;
            items = new List<GroupItem>();
        }

        public void AddRange(List<GroupItem> items)
        {
            this.items.AddRange(items);
            this.NotifyDataSetChanged();
        }
        public void Clear()
        {
            this.items= new List<GroupItem>(); 
            this.NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
            Inflate(Resource.Layout.vh_searchGroupltem, parent, false);
            FoundGroupViewHolder vh = new FoundGroupViewHolder(itemView);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            FoundGroupViewHolder vh = holder as FoundGroupViewHolder;
            vh.Name.Text = items[position].Name;

            ImageService.Instance.LoadUrl(items[position].Photo)
            .Retry(2, 200)
            .LoadingPlaceholder("ic_done", FFImageLoading.Work.ImageSource.CompiledResource)
            .ErrorPlaceholder("ic_action", FFImageLoading.Work.ImageSource.CompiledResource)
            .Error(e => System.Diagnostics.Debug.WriteLine(e.Message)) //TODO: log                                 
            .IntoAsync(vh.Image);

            vh.AddButton.Tag = position;

            if (!items[position].IsExist)
            {
                vh.AddButton.Visibility = ViewStates.Visible;

                if (!vh.AddButton.HasOnClickListeners)
                    vh.AddButton.Click += async (s, e) =>
                {
                    vh.AddButton.Visibility = ViewStates.Gone;
                    //TODO переписать
                    try
                    {
                        //load 
                        await GroupsSearchHelper.SaveNewItem(items[position], ((MainActivity)_activity).Connection);
                    }
                    catch (Exception ex)
                    {
                        vh.AddButton.Visibility = ViewStates.Visible;
                        //TODO:log
                    }
                };
            }
        }

        public override int ItemCount
        {
            get { return items.Count; }
        }

        public class FoundGroupViewHolder : RecyclerView.ViewHolder
        {
            [InjectView(Resource.Id.imgView_imageFoundGroup)]
            public ImageViewAsync Image { get; private set; }
            [InjectView(Resource.Id.txtView_nameFoundGroup)]
            public TextView Name { get; private set; }
            [InjectView(Resource.Id.imgBtn_addToDownload)]
            public ImageButton AddButton { get; private set; }

            public FoundGroupViewHolder(View view) : base(view)
            {
                Cheeseknife.Inject(this, view);
                AddButton.Visibility = ViewStates.Gone;
            }
        }
    }    
}
