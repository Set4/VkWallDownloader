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
using Com.Xenione.Libs.Swipemaker;

namespace VkPostDownloader
{
    [Activity(Label = "SwipeMaker")]
    public class SwipeMaker : AbsCoordinatorLayout
    {        
        private View mBackgroundView;
        private SwipeLayout mForegroundView;

        public SwipeMaker(Context context) : base(context)
        {
        }

        public SwipeMaker(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public SwipeMaker(Context context, IAttributeSet attrs,int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public SwipeMaker(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public override void DoInitialViewsLocation()
        {
            mBackgroundView = FindViewById(Resource.Id.item_content);
            mForegroundView = FindViewById<SwipeLayout>(Resource.Id.foregroundView);
            mForegroundView.Anchor(Java.Lang.Integer.ValueOf(-mBackgroundView.Width), Java.Lang.Integer.ValueOf(0), Java.Lang.Integer.ValueOf(mBackgroundView.Left));

        }

        public override void OnTranslateChange(float global, int index, float relative)
        {
            mBackgroundView.Alpha = global;
            mForegroundView.Alpha = 1 - global;
        }       
    }
}