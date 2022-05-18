namespace WhatsappWebServer
{
    public class User
    {
        public string userNameId { get; set; }
        public string password { get; set; }

        public string displayName { get; set; }

        public List<Chat> chats { get; set; }
    }
}
