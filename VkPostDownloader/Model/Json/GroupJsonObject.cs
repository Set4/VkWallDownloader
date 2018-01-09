namespace VkPostDownloader.Model.Json.Group
{
    public class Rootobject
    {
        public Response response { get; set; }
    }

    public class Response
    {
        public int count { get; set; }
        public GroupJsonModel[] items { get; set; }
    }

    public class GroupJsonModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public int is_closed { get; set; }
        public string type { get; set; }
        public int is_admin { get; set; }
        public int is_member { get; set; }
        public string photo_50 { get; set; }
        public string photo_100 { get; set; }
        public string photo_200 { get; set; }
    }

    public class SearchGroupResponse
    {
        public GroupJsonModel[] response { get; set; }
    }
}