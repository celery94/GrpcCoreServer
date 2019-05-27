using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ApiAuth;
using Grpc.Core;
using GrpcCoreClient.Code;

namespace GrpcCoreClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //HttpClient httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri("https://localhost:50051");
            //var result = await httpClient.PostAsync("api/token", new { Email = "admin@contract.com", Password = "12345678" }.AsJson());
            var tokenValue = "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJiNjg4MGRkZi00MmJlLTQyMmQtOWM0Mi0xZTNiYmExY2MxNGUiLCJuYmYiOjE1NTg5NDIwNjAsImV4cCI6MTU1OTU0Njg2MCwiaWF0IjoxNTU4OTQyMDYwfQ.eLmPKXz0837YvhqpxaWnveyiSAwDHmBsTewrG8FwFrs";

            var metadata = new Metadata
            {
                { "Authorization", tokenValue }
            };
            CallOptions callOptions = new CallOptions(metadata);

            var channelCredentials = new SslCredentials(
                File.ReadAllText("Certs\\ca.crt"),
                new KeyCertificatePair(
                    File.ReadAllText("Certs\\client.crt"),
                    File.ReadAllText("Certs\\client.key")
                )
            );

            var channel = new Channel("localhost:50051", channelCredentials);

            var client = new Greeter.GreeterClient(channel);

            var user = "you";

            var reply = client.SayHello(new HelloRequest { Name = user }, callOptions);
            //var reply = client.SayHello(new HelloRequest { Name = user });
            Console.WriteLine("Greeting: " + reply.Message);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}