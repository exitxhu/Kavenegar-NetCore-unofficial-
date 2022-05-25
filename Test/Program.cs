// See https://aka.ms/new-console-template for more information
using Kavenegar_NetCore_unofficial_;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");
var serviceProvider = new ServiceCollection()
            .AddKavenegar(a =>
            {
                a.ApiKey = "ApiKey";
                a.BaseUrl = "https://api.kavenegar.com/v1";
            })
            .BuildServiceProvider();

var ser = serviceProvider.GetRequiredService<KavenegarService>();
var t = await ser.Send(new string[] { "091211111111" }, "دلام", "10004346");