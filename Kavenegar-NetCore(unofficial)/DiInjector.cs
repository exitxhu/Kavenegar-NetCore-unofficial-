using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kavenegar_NetCore_unofficial_
{
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
            services.AddHttpClient<KavenegarHttpService>(cl => cl.BaseAddress = conf.GetUri());
            return services;

        }
    }

}