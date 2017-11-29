using Android.App;
using Android.Content;

namespace VkPostDownloader
{
    [Service]
    public class GroupWallDownlodaderIntentService : IntentService
    {
        public GroupWallDownlodaderIntentService() : base("DemoIntentService")
        {
        }

        protected override void OnHandleIntent(Intent intent)
        {
            int idGroup = intent.GetIntExtra("id", 0);
            
            //TODO: long download walls 
        }
    }
}