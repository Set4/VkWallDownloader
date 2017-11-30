using FFImageLoading;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VKontakte;
using VKontakte.API;
using VkPostDownloader.Model;
using VkPostDownloader.Model.Json.Group;
using VkPostDownloader.Model.Json.Wall;
using VkPostDownloader.UtilityClasses.CommonClasses;

namespace VkPostDownloader.UtilityClasses
{
    static class GroupsSearchHelper
    {
        public static Task<List<GroupItem>> GetSearchResult(string query, int count, int offset, SQLiteAsyncConnection connection)
        {
            return Task.Run(async () =>
            {
                List<GroupItem> items = new List<GroupItem>();
                try
                {
                    GroupItem itemGroup;

                    var request = VKApi.Groups.Search(VKParameters.From(VKApiConst.Q, query, 
                        VKApiConst.Offset, offset,
                        VKApiConst.Count, count, "type", "group, page", 
                        VKApiConst.AccessToken, VKSdk.AccessToken));
                    var response = await request.ExecuteAsync();
                    var data = JsonConvert.DeserializeObject<Model.Json.Group.Rootobject>(response.Json.ToString());

                    if (data.response.count > 0)
                        foreach (var item in RemoveCloseGroups(data.response.items))
                        {
                            itemGroup = ConvertHelper.ConvertToGroupItem(item);
                            itemGroup.IsExist = await DbHelper.CheckIsExist(itemGroup.Id, connection);
                            items.Add(itemGroup);
                        }
                }
                catch (Exception ex)
                {
                    //ToDO: log
                }

                return items;
            });
        }

        public static async Task SaveNewItem(GroupItem item, SQLiteAsyncConnection connection)
        {
            await Task.Run(async () =>
            {
                int groupItemKey = await DbHelper.Insert<GroupItem>(item, connection);

                await ImageService.Instance.LoadUrl(item.Photo)
               .Success((imageInfo, result) =>
               {
                   item.PhotoPath = imageInfo.FilePath;
               })
               .Retry(2, 200)
               .Error((ex) => System.Diagnostics.Debug.WriteLine(ex.Message)) //TODO: log        
               .DownloadOnlyAsync();

                //load posts
                int countWall = 0;
                int countLoadetWall = 0;
                do
                {
                    var request = VKApi.Wall.Get(VKParameters.From(VKApiConst.Q, "-" + item.Id, VKApiConst.AccessToken, VKSdk.AccessToken, VKApiConst.Count, 100, VKApiConst.Offset, countLoadetWall));

                    var response = await request.ExecuteAsync();
                    var data = JsonConvert.DeserializeObject<Model.Json.Wall.Rootobject>(response.Json.ToString());

                    countWall = data.response.count;

                    foreach (var i in data.response.items)
                    {
                        PostItem wall = new PostItem()
                        {
                            Id = i.id,
                            GroupItemKey = groupItemKey,
                            Text = i.text
                        };

                        int postItemKey = await DbHelper.Insert<PostItem>(wall, connection);

                        foreach (var image in i.attachments.Where(att => att.type == "photo"))
                        {
                            if (image.photo != null)
                            {
                                await ImageService.Instance.LoadUrl(item.Photo)
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

            return;
        }

        private static IEnumerable<GroupJsonModel> RemoveCloseGroups(IEnumerable<GroupJsonModel> items)
        {
            return items.Where(i => i.is_closed == 0 || i.is_admin == 1 || i.is_member == 1);
        }
    }
}