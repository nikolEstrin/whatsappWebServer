using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhatsappWebServer;

namespace WhatsappWebServer.Data
{
    public class WhatsappWebServerContext : DbContext
    {
        public WhatsappWebServerContext (DbContextOptions<WhatsappWebServerContext> options)
            : base(options)
        {
        }

        public DbSet<WhatsappWebServer.Message>? Message { get; set; }

        public DbSet<WhatsappWebServer.User>? User { get; set; }

        public DbSet<WhatsappWebServer.Contact>? Contact { get; set; }
    }
}
