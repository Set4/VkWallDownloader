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
using SQLite;
using System.Threading.Tasks;
using System.IO;

namespace VkPostDownloader
{
    //
    static class DbHelper
    {
        public static async Task<int> Insert<T>(T item, SQLiteAsyncConnection connection)where T: IItemModel
        {
            return await connection.InsertAsync(item);
        }

        public static async Task<bool> CheckIsExist<T>(int id, SQLiteAsyncConnection connection) where T : IItemModel, new()
        {
            T item = await connection.Table<T>().Where(i => i.Id == id).FirstOrDefaultAsync();
            return item == null ? false : true;
        }

        public static async Task<IEnumerable<T>> GetAllItems<T>(SQLiteAsyncConnection connection) where T : IItemModel, new()
        {
           return await  connection.Table<T>().ToListAsync();
        }

        public static async Task<IEnumerable<T>> GetItems<T>(SQLiteAsyncConnection connection, string query, params object[] args) where T : IItemModel, new()
        {
            return await connection.QueryAsync<T>(query, args);
        }
      

        public static async Task<int> GetCountWalls(int groupItemKey, SQLiteAsyncConnection connection) 
        {
            return await connection.Table<PostItem>().Where(i=>i.GroupItemKey== groupItemKey).CountAsync();
        }

        public static async Task ClearGroupItem(GroupItem item, SQLiteAsyncConnection connection)
        {

          await  connection.RunInTransactionAsync( tran => {

                try
                {
                  if(File.Exists(item.PhotoPath))
                  {
                      File.Delete(item.PhotoPath);
                  }

                  List<PostItem> posts = tran.Table<PostItem>().Where(i => i.GroupItemKey == item.Key).ToList();

                  tran.Delete(item);

                    foreach(var p in posts)
                    {
                      foreach (var img in tran.Table<ImageItem>().Where(i=>i.PostItemKey==p.Key))
                      {                         
                          if (File.Exists(img.ImagePath))
                              File.Delete(img.ImagePath);

                          tran.Delete(img);
                      }
                          tran.Delete(p);
                    }
                   
                    tran.Commit();
                }
                catch(Exception ex)
                {
                    throw new Exception("Error DeleteGroupItem", ex);
                }
            });

            return;
        }

        public static async Task ClearPosts(int groupItemKey, SQLiteAsyncConnection connection)
        {
            await connection.RunInTransactionAsync(tran => {

                try
                {

                   
                    List<PostItem> posts = tran.Table<PostItem>().Where(i => i.GroupItemKey == groupItemKey).ToList();

                   
                    foreach (var p in posts)
                    {
                        foreach (var img in tran.Table<ImageItem>().Where(i => i.PostItemKey == p.Key))
                        {
                            if (File.Exists(img.ImagePath))
                                File.Delete(img.ImagePath);

                            tran.Delete(img);
                        }
                        tran.Delete(p);
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DeleteGroupItem", ex);
                }
            });

            return;
        }


    }
}