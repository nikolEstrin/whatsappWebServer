using System.ComponentModel.DataAnnotations;

namespace WhatsappWebServer
{
    public class User
    {
        public string Id { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        public string displayName { get; set; }

        public List<Chat> chats { get; set; }
    }
}
