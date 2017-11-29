using System;
using Android.Support.V7.Widget;

namespace VkPostDownloader.UtilityClasses
{
    class EndlessLoadOnScrollListener : RecyclerView.OnScrollListener
    {
        private LinearLayoutManager _layoutManager;
        private int _maxCountItems;

        public delegate void LoadMoreEventHandler(object sender, EventArgs e);
        public event LoadMoreEventHandler LoadMoreEvent;
        public bool IsLoading { get; private set; }

        public EndlessLoadOnScrollListener(LinearLayoutManager layoutManager, int maxCountItems)
        {
            _layoutManager = layoutManager;
            this._maxCountItems = maxCountItems;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            var visibleItemCount = recyclerView.ChildCount;
            var totalItemCount = recyclerView.GetAdapter().ItemCount;
            var pastVisiblesItems = _layoutManager.FindFirstVisibleItemPosition();

            if ((visibleItemCount + pastVisiblesItems) >= totalItemCount && !IsLoading && totalItemCount <= _maxCountItems)
            {
                IsLoading = true;
                LoadMoreEvent(this, null);
                IsLoading = false;
            }
        }
    }
}