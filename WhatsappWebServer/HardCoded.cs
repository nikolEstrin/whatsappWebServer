namespace WhatsappWebServer
{
    public static class HardCoded
    {
        public static List<Message> nikolAliceMessages = new List<Message> { new Message() { id = 181, content = "How are you?", created = "2022-05-20T19:46:00", sent = false },
       new Message() { id = 183, content = "ok", created = "2022-05-20T19:47:00", sent = true },
       new Message() { id = 184, content = "I know what you did last summer", created = "2022-05-20T20:00:00", sent = true }};

        public static List<Message> nikolBobMessages = new List<Message> { new Message() { id = 1, content = "dont kill me", created = "2022-05-20T19:46:00", sent = false },
       new Message() { id = 2, content = "any last words?", created = "2022-05-20T20:00:00", sent = true }};

        public static List<Message> aliceNikolMessages = new List<Message> { new Message() { id = 11, content = "How are you?", created = "2022-05-20T19:46:00", sent = true },
       new Message() { id = 12, content = "ok", created = "2022-05-20T19:47:00", sent = false },
       new Message() { id = 13, content = "I know what you did last summer", created = "2022-05-20T20:00:00", sent = false }};

        public static List<Message> bobNikolMessages = new List<Message> { new Message() { id = 1, content = "dont kill me", created = "2022-05-20T19:46:00", sent = true },
       new Message() { id = 2, content = "any last words?", created = "2022-05-20T20:01:00", sent = false }};

        public static List<Chat> nikolChats = new List<Chat>() { new Chat() { id="Alice", messages=nikolAliceMessages},
       new Chat() { id="Bob", messages=nikolBobMessages}};

        public static List<Chat> aliceChats = new List<Chat>() { new Chat() { id = "Nikol", messages = aliceNikolMessages } };

        public static List<Chat> bobChats = new List<Chat>() { new Chat() { id = "Nikol", messages = bobNikolMessages } };

        public static List<Chat> charlieChats = new List<Chat>();

        public static List<Contact> nikolContacts = new List<Contact> { new Contact() { id = "Alice", name = "Alicia", server = "localhost:7132", last = "I know what you did last summer", lastdate="2022-05-20T20:00:00" },
       new Contact() { id = "Bob", name = "Bobby", server = "localhost:7266", last = "any last words?", lastdate="2022-05-20T20:00:00" }};

        public static List<Contact> aliceContacts = new List<Contact> { new Contact() { id = "Nikol", name = "Nik", server = "localhost:7132", last = "I know what you did last summer", lastdate = "2022-05-20T20:00:00" } };

        public static List<Contact> bobContacts = new List<Contact> { new Contact() { id = "Nikol", name = "Nik", server = "localhost:7132", last = "any last words?", lastdate = "2022-05-20T20:00:00" }, };

        public static List<Contact> charlieContacts = new List<Contact>();

        public static List<User> users = new List<User> { new User() { Id = "Nikol", displayName = "Nik", password = "Nn1", contacts = nikolContacts, chats = nikolChats },
       new User() { Id = "Alice", displayName = "Alicia", password = "Aa1", contacts = aliceContacts, chats = aliceChats },
       new User() { Id = "Bob", displayName = "Bobby", password = "Bb1", contacts = bobContacts, chats = bobChats },
       new User() { Id = "Charlie", displayName = "Charlie", password = "Cc1", contacts = charlieContacts, chats = charlieChats }};

    }
}
