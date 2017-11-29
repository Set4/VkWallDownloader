using SQLite;

namespace VkPostDownloader.Model
{
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
        public string PhotoPath { get; set; }
        [Ignore]
        public bool IsExist { get; set; }

        public GroupItem()
        {
        }
    }
}