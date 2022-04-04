using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.AutoInvestManager.Client;
using Service.AutoInvestManager.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            // var factory = new AutoInvestManagerClientFactory("http://localhost:5001");
            // var client = factory.GetHelloService();
            //
            // var resp = await  client.SayHelloAsync(new CreateInstructionRequest(){Name = "Alex"});
            // Console.WriteLine(resp?.Message);

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
