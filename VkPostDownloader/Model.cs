using SQLite;
using System.Collections.Generic;

namespace VkPostDownloader
{
    public interface IItemModel
    {
        int Key { get; set; }
        int Id { get; set; }
    }

    public class GroupItem : IItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public string Type { get; set; }
        public string Photo { get; set; }
        public int CountPosts { get; set; }       

        [Ignore]
        public bool IsExist { get; set; }


        public string PhotoPath { get; set; }

        public GroupItem()
        {
        }
    }

    public class PostItem : IItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int Id { get; set; }
        [Indexed]
        public int GroupItemKey { get; set; }

        public string Text { get; set; }
        public List<ImageItem> Image { get; set; } = new List<ImageItem>();
    }

    public class ImageItem : IItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int Id { get; set; }
        [Indexed]
        public int PostItemKey { get; set; }

        public string ImagePath { get; set; }
    }

}