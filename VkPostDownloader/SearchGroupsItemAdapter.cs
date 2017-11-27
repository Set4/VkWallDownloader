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
using FFImageLoading;
using FFImageLoading.Views;
using System.Threading.Tasks;
using System.IO;
using VKontakte.API;
using VKontakte;
using Newtonsoft.Json;

namespace VkPostDownloader
{

    class SearchGroupsItemAdapter : RecyclerView.Adapter
    {
        public List<GroupItem> items;
        Activity activity;
        public SearchGroupsItemAdapter(Activity activity)
        {
            this.activity = activity;
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
                       // await ((MainActivity)activity).Connection.RunInTransactionAsync

                     
                        //load 
                         await Task.Run(async () =>
                        {
                            int groupItemKey =await DbHelper.Insert<GroupItem>(items[position], ((MainActivity)activity).Connection);

                            await ImageService.Instance.LoadUrl(items[position].Photo)
                           .Success((imageInfo, result) =>
                           {
                               items[position].PhotoPath = imageInfo.FilePath;
                           })
                           .Retry(2, 200)
                           .Error((ex) => System.Diagnostics.Debug.WriteLine(ex.Message)) //TODO: log        
                           .DownloadOnlyAsync();

                            //load posts
                            int countWall = 0;
                            int countLoadetWall = 0;
                            do
                            {
                                var request= VKApi.Wall.Get(VKParameters.From(VKApiConst.Q, "-" + items[position].Id, VKApiConst.AccessToken, VKSdk.AccessToken, VKApiConst.Count, 100, VKApiConst.Offset, countLoadetWall));

                                var response = await request.ExecuteAsync();
                                var data = JsonConvert.DeserializeObject<Rootobject>(response.Json.ToString());

                                countWall = data.response.count;

                                foreach (var i in data.response.items)
                                {
                                    PostItem wall = new PostItem()
                                    {
                                        Id = i.id,
                                        GroupItemKey = groupItemKey,
                                        Text = i.text
                                    };

                                    int postItemKey= await DbHelper.Insert<PostItem>(wall, ((MainActivity)activity).Connection);

                                    foreach (var image in i.attachments.Where(att => att.type == "photo"))
                                    {
                                        if (image.photo != null)
                                        {

                                            await ImageService.Instance.LoadUrl(items[position].Photo)
                                           .Success((imageInfo, result) =>
                                           {
                                               var imageItem = new ImageItem()
                                               {
                                                   Id = image.photo.id,
                                                   PostItemKey = postItemKey,
                                                   ImagePath = imageInfo.FilePath
                                               };

                                           })
                                           .Retry(2, 200)
                                           .Error((ex) => System.Diagnostics.Debug.WriteLine(ex.Message)) //TODO: log        
                                           .DownloadOnlyAsync();



                                        }
                                    }
                                    
                                    countLoadetWall++;
                                }
       

                            }
                            while (countWall > countLoadetWall);




                        });

                    }
                    catch (Exception ex)
                    {
                        vh.AddButton.Visibility = ViewStates.Visible;
                        //TODO:log
                    }


                 

                   // ((MainActivity)activity).StartService(((MainActivity)activity).DownloadIntent.PutExtra("id", items[(int)((ImageButton)s).Tag].Id));

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

    class EndlessLoadOnScrollListener : RecyclerView.OnScrollListener
    {
        public delegate void LoadMoreEventHandler(object sender, EventArgs e);
        public event LoadMoreEventHandler LoadMoreEvent;
        public bool IsLoading { get; private set; }

        private LinearLayoutManager LayoutManager;
        private int maxCountItems;

        public EndlessLoadOnScrollListener(LinearLayoutManager layoutManager, int maxCountItems)
        {
            LayoutManager = layoutManager;
            this.maxCountItems = maxCountItems;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            var visibleItemCount = recyclerView.ChildCount;
            var totalItemCount = recyclerView.GetAdapter().ItemCount;
            var pastVisiblesItems = LayoutManager.FindFirstVisibleItemPosition();

            if ((visibleItemCount + pastVisiblesItems) >= totalItemCount && !IsLoading && totalItemCount <= maxCountItems)
            {
                IsLoading = true;
                LoadMoreEvent(this, null);
                IsLoading = false;
            }
        }
    }
}
