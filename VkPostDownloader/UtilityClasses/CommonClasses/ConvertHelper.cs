using VkPostDownloader.Model;
using VkPostDownloader.Model.Json.Group;

namespace VkPostDownloader.UtilityClasses.CommonClasses
{
    static class ConvertHelper
    {
        public static GroupItem ConvertToGroupItem(GroupJsonModel item)
        {
            return new GroupItem()
            {
                Id = item.id,
                Name = item.name,
                Photo = item.photo_100,
                ScreenName = item.screen_name,
                Type = item.type
            };
        }
    }
}