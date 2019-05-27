using System;
using System.IO;
using ApiAuth;
using Grpc.Core;

namespace GrpcCoreClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
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

            var reply = client.SayHello(new HelloRequest {Name = user});
            Console.WriteLine("Greeting: " + reply.Message);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}