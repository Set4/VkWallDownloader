using SQLite;

namespace VkPostDownloader.Model
{
    public class ImageItem : IItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int Key { get; set; }
        public int Id { get; set; }
        public string ImagePath { get; set; }
        [Indexed]
        public int PostItemKey { get; set; }
    }
}