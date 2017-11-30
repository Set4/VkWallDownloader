using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using FFImageLoading;
using VkPostDownloader.Model;

namespace VkPostDownloader
{
    class WallsItemAdapter : RecyclerView.Adapter
    {
        private Activity _activity;

        public List<PostItem> items;

        public WallsItemAdapter(Activity activity)
        {
            _activity = activity;
            items = new List<PostItem>();
        }

        public void AddRange(List<PostItem> items)
        {
            this.items.AddRange(items);
            this.NotifyDataSetChanged();
        }
        public void Clear()
        {
            this.items = new List<PostItem>();
            this.NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
            Inflate(Resource.Layout.vh_wallltem, parent, false);
            WallViewHolder vh = new WallViewHolder(itemView);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            WallViewHolder vh = holder as WallViewHolder;

            TextView txt = new TextView(_activity.BaseContext);
            txt.Text = items[position].Text;
          
            txt.LayoutParameters
                = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);

            vh.Layout.AddView(txt);

            for (int i = 0; i < items[position].Image.Count(); i++)
            {
                FFImageLoading.Views.ImageViewAsync img = new FFImageLoading.Views.ImageViewAsync(_activity.BaseContext);
              
                img.LayoutParameters
                = new ViewGroup.LayoutParams(96, 96);
                img.SetScaleType(ImageView.ScaleType.CenterCrop);

                ImageService.Instance.LoadFile(items[position].Image[i].ImagePath)
                  .LoadingPlaceholder("ic_done", FFImageLoading.Work.ImageSource.CompiledResource)
                  .ErrorPlaceholder("ic_action", FFImageLoading.Work.ImageSource.CompiledResource)
                  .Error(e => System.Diagnostics.Debug.WriteLine(e.Message)) //TODO: log                                 
                  .IntoAsync(img);

                vh.Layout.AddView(img);
            }
        }

        public override int ItemCount
        {
            get { return items.Count; }
        }

        class WallViewHolder : RecyclerView.ViewHolder
        {           
            public LinearLayout Layout { get; private set; }

            public WallViewHolder(View view) : base(view)
            {
                Layout = view.FindViewById<LinearLayout>(Resource.Id.linLayout_contanerwall);
            }
        }
    }
}