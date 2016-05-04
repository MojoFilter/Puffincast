using Puffincast.Models;
using Puffincast.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Puffincast.Controllers
{
    public class PuffController : ApiController
    {
        private ICommandHandler commandHandler;
        private ISettingsProvider settings;

        public PuffController() : this(null) { }

        public PuffController(ISettingsProvider settings, ICommandHandler commandHandler = null,
            IWinampControl control = null)
        {
            this.settings = settings ?? new SettingsProvider();
            this.commandHandler = commandHandler ?? new CommandHandler(this.settings);
        }

        public async Task<HttpResponseMessage> Get(string user_name, string text)
        {
            var result = await this.commandHandler.Execute(user_name, text);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(result);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }
    }
}
