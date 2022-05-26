using Microsoft.AspNetCore.Mvc;
using WhatsappWebServer.Services;

namespace WhatsappWebServer.Controllers
{
    [ApiController]
    [Route("api/transfer")]
    public class TransferController : Controller
    {
        private readonly ITransferService _service;

        public TransferController(ITransferService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Index([Bind("from,to,content")] Connection connection)
        {
            return _service.Index(connection);
        }
    }
}
