using System.Collections.Generic;
using SQLite;

namespace VkPostDownloader.Model
{
    public class PostItem : IItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int Id { get; set; }
        [Indexed]
        public int GroupItemKey { get; set; }

        public string Text { get; set; }
        [Ignore]
        public List<ImageItem> Image { get; set; } = new List<ImageItem>();
    }
}