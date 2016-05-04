using Puffincast.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Puffincast.Controllers
{
    public class PuffController : ApiController
    {
        private ICommandHandler commandHandler;

        public PuffController() : this(null) { }

        public PuffController(ICommandHandler commandHandler)
        {
            this.commandHandler = commandHandler ?? new CommandHandler();
        }

        public async Task<string> Get(string user_name, string text)
        {
            return await this.commandHandler.Execute(user_name, text);
        }
    }
}
