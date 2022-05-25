using Microsoft.Extensions.Options;
using System.Text;
using Newtonsoft.Json;

namespace Kavenegar_NetCore_unofficial_
{
    public class KavenegarService
    {
        private readonly HttpClient _httpClient;
        private readonly KavenegarConfig _config;

        public KavenegarService(HttpClient httpClient, IOptions<KavenegarConfig> options)
        {
            _httpClient = httpClient;
            this._config = options.Value;
        }
        public async Task<ReturnSend> Send(IEnumerable<string> recievers, string message, string sender, MessageType? type = MessageType.MobileMemory, DateTime? date = null, List<string> localids = null)
        {
            List<KeyValuePair<string, string>> param = ParamMaker(recievers, message, sender, type, date);
            return await Call(param, "/sms/send.json");
        }

        private static List<KeyValuePair<string, string>> ParamMaker(IEnumerable<string> recievers, string message, string sender, MessageType? type, DateTime? date)
        {
            var res = new List<KeyValuePair<string, string>>
            {
            new("sender", System.Net.WebUtility.HtmlEncode(sender)),
            new("receptor", System.Net.WebUtility.HtmlEncode(string.Join(",", recievers))),
            new("message", message) 
            };
            if (type is not null)
                res.Add(new("type", type.ToString()!));
            if (date is not null)
                res.Add(new("date", date.DateTimeToUnixTimestamp().ToString()));
            return res;
        }

        private async Task<ReturnSend> Call(List<KeyValuePair<string, string>> param, string endpoint)
        {
            var sendUri = new Uri(_config.GetUri() + endpoint);
            var postdata = new FormUrlEncodedContent(param);
            var res = await _httpClient.PostAsync(sendUri, postdata);
            var tx = await res.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ReturnSend>(tx);
            return resp;
        }
    }
 
}