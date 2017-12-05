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
using FloatingSearchViews;

namespace VkWallReader
{
    public class SearchFragment : Fragment
    {
        [InjectView(Resource.Id.floating_search_view)]
        FloatingSearchView searctToolbar;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.search_layout, null);
            Cheeseknife.Inject(this, view);

            searctToolbar.QueryChange += async (sender, e) => {
                if (!string.IsNullOrEmpty(e.OldQuery) && string.IsNullOrEmpty(e.NewQuery))
                {
                    searctToolbar.ClearSuggestions();
                }
                else
                {
                    // show the top left circular progress
                    searctToolbar.ShowProgress();

                    // simulates a query call to a data source with a new query.
                    var results = e.NewQuery;

                    // swap the data and collapse/expand the dropdown
                   // searctToolbar.SwapSuggestions(results);

                    // complete the progress
                    searctToolbar.HideProgress();
                }
            };

            searctToolbar.MenuItemClick += (sender, e) => {
                var item = e.MenuItem;
            };

            return view;
        }
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            Cheeseknife.Reset(this);
        }
    }
}