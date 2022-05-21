namespace WhatsappWebServer
{
    public class Message
    {
        public int? id { get; set; }

        public string content { get; set; }

        public string? created { get; set; }

        public bool? sent { get; set; }
    }
}
