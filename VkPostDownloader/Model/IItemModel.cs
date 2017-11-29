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

namespace VkPostDownloader.Model
{
    public interface IItemModel
    {
        int Key { get; set; }
        int Id { get; set; }
    }
}