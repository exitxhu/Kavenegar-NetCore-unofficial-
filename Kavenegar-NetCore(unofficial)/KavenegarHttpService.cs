using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace Kavenegar_NetCore_unofficial_
{
    public class KavenegarHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<KavenegarConfig> _options;

        public KavenegarHttpService(HttpClient httpClient, IOptions<KavenegarConfig> options)
        {
            _httpClient = httpClient;
            this._options = options;
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
            var sendUri = new Uri(_httpClient.BaseAddress + endpoint);
            var postdata = new FormUrlEncodedContent(param);
            var res = await _httpClient.PostAsync(sendUri, postdata);
            var tx = await res.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ReturnSend>(tx);
            return resp;
        }
    }
    internal static class Helpers
    {
        public static long DateTimeToUnixTimestamp(this DateTime? dateTime)
        {
            if (!dateTime.HasValue) return 0;
            try
            {
                var idateTime = dateTime.Value;
                idateTime = new DateTime(idateTime.Year, idateTime.Month, idateTime.Day, idateTime.Hour, idateTime.Minute, idateTime.Second);
                TimeSpan unixTimeSpan = (idateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).ToLocalTime());
                return long.Parse(unixTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            }
            catch
            {
                return 0;
            }
        }
        public static DateTime UnixTimestampToDateTime(this long unixTimeStamp)
        {
            try
            {
                return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(unixTimeStamp);
            }
            catch
            {
                return DateTime.MaxValue;
            }
        }
    }
    public static class DiInjector
    {
        public static IServiceCollection AddKavenegar(this IServiceCollection services, Action<KavenegarConfig> kavenegarConfiguration)
        {
            var kav = new KavenegarConfig();
            kavenegarConfiguration(kav);

            services
                .Configure(kavenegarConfiguration)
                .AddShared(kav);
            return services;
        }
        public static IServiceCollection AddKavenegar(this IServiceCollection services, IConfigurationSection kavenegarConfigurationSection)
        {
            var kav = kavenegarConfigurationSection.Get<KavenegarConfig>();

            services
                .Configure<KavenegarConfig>(kavenegarConfigurationSection)
                .AddShared(kav);
            return services;
        }
        static IServiceCollection AddShared(this IServiceCollection services, KavenegarConfig conf)
        {
            services.AddHttpClient<KavenegarHttpService>(cl => cl.BaseAddress = new Uri($"{conf.BaseUrl}/{conf.ApiKey}"));
            return services;

        }
    }
    public class KavenegarConfig
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
    }
}