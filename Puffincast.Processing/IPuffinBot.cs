using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Processing
{
    public interface IPuffinBot
    {
        Task NotifyEnqueue(string user, string track);
    }

    public class PuffinBot : IPuffinBot
    {
        private static readonly string HookUri = "https://hooks.slack.com/services/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";

        public async Task NotifyEnqueue(string user, string track)
        {
            var message = $"{user} just loaded up _{track}_";
            var request = WebRequest.CreateHttp(HookUri);
            request.Method = "POST";
            var payload = new { text = message };
            var body = JsonConvert.SerializeObject(payload);
            var content = Encoding.UTF8.GetBytes(body);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = content.Length;
            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(content, 0, content.Length);
            }
            await request.GetResponseAsync();
        }
    }
}
