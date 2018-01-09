using Android.App;
using Android.Content;
using SQLite;
using System;
using System.IO;
using VkPostDownloader.UtilityClasses;

namespace VkPostDownloader
{
    [Service]
    public class GroupWallDownlodaderIntentService : IntentService
    {
        public SQLiteAsyncConnection Connection { get; private set; }
        string dataBasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "appDb.db3");
        public GroupWallDownlodaderIntentService() : base("DemoIntentService")
        {
            
        }

        public async override void OnCreate()
        {
            base.OnCreate();
            Connection = await DbHelper.CreateConnection(dataBasePath);
            Console.WriteLine("IntentService OnCreate");
        }

        protected async override void OnHandleIntent(Intent intent)
        {
            int idGroup = intent.GetIntExtra("id", 0);



            Console.WriteLine("IntentService onHandleIntent start {0}",idGroup);
            //TODO: long download walls 

            await GroupsSearchHelper.SaveNewItem(await GroupsSearchHelper.GetGroupItemById(idGroup), Connection);

            Console.WriteLine("IntentService onHandleIntent end {0}", idGroup);
        }


        public override void OnDestroy()
        {
            base.OnDestroy();
            Console.WriteLine("IntentService onDestroy");
        }



    }
}