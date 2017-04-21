using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using static Messages.EmployeeService;

namespace GrpcClient
{
    public class GrpcClient
    {
        private Channel _channel;

        public GrpcClient()
        {
            const int Port = 555;
            _channel = new Channel("localhost", Port, ChannelCredentials.Insecure);
        }

        public async Task GetByBadgeNumber()
        {
            var res = await new DefaultCallInvoker(_channel)
                    .AsyncUnaryCall(__Method_GetByBadgeNumber, null,
                        new CallOptions(),
                        new GetByBadgeNumberRequest() { BadgeNumber = 2080 });
            Console.WriteLine($"\n\ngetbybadge number {res.Employee}");
        }

        public async Task GetAll()
        {
            using (var call =
                new DefaultCallInvoker(_channel).AsyncServerStreamingCall(__Method_GetAll, null, new CallOptions(),
                    new GetAllRequest()))
            {
                var responseStream = call.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    Console.WriteLine($"getting: {responseStream.Current.Employee.LastName}");
                }
            }
        }

        public async Task AddPhoto()
        {
            Metadata md = new Metadata();
            md.Add("badgenumber", "2080");

            FileStream fs = File.OpenRead("Penguins.jpg");
            using (var call = new DefaultCallInvoker(_channel)
                    .AsyncClientStreamingCall(__Method_AddPhoto, null, new CallOptions()))
            {
                var stream = call.RequestStream;
                while (true)
                {
                    byte[] buffer = new byte[64 * 1024];
                    int numRead = await fs.ReadAsync(buffer, 0, buffer.Length);
                    if (numRead == 0)
                    {
                        break;
                    }
                    if (numRead < buffer.Length)
                    {
                        Array.Resize(ref buffer, numRead);
                    }

                    await stream.WriteAsync(new AddPhotoRequest()
                    {
                        Data = buffer
                    });
                }
                await stream.CompleteAsync();

                var res = await call.ResponseAsync;

                Console.WriteLine(res.IsOk);
            }
        }

        public async Task PostRate()
        {
            var res = await new DefaultCallInvoker(_channel)
                    .AsyncUnaryCall(__Method_PostRate, null,
                        new CallOptions(),
                        new RateMessageRequest() {
                            Symbol = "abc",
                            Ask =20m});
            Console.WriteLine(res);
        }

        public async Task SendMetadataAsync()
        {
            Metadata md = new Metadata();
            md.Add("username", "mvansickle");
            md.Add("password", "password1");
            try
            {
                await new DefaultCallInvoker(_channel)
                    .AsyncUnaryCall(__Method_GetByBadgeNumber, null,
                        new CallOptions().WithHeaders(md),
                        new GetByBadgeNumberRequest());
            }
            catch (Exception e) {
            }
            Console.WriteLine("\n\n Metadata");
        }

        public async Task SaveAll()
        {
            var employees = new List<Employee>()
            {
                new Employee{
                    BadgeNumber= 123,
                    FirstName= "John",
                    LastName= "Smith",
                    VacationAccrualRate= 1.2f,
                    VacationAccrued= 0,
                },
                new Employee{
                    BadgeNumber= 234,
                    FirstName= "Lisa",
                    LastName= "Wu",
                    VacationAccrualRate= 1.7f,
                    VacationAccrued= 10,
                }
            };
            using (var call = new DefaultCallInvoker(_channel)
                .AsyncDuplexStreamingCall(__Method_SaveAll, null, new CallOptions()))
            {
                var requestStream = call.RequestStream;
                var responseStream = call.ResponseStream;

                var responseTask = Task.Run(async () =>
                {
                    while (await responseStream.MoveNext())
                    {
                        var emp = responseStream.Current.Employee;
                        Console.WriteLine(
                            $"{emp.BadgeNumber},{emp.FirstName}");
                    }
                });

                foreach (var e in employees)
                {
                    await requestStream.WriteAsync(new EmployeeRequest() { Employee = e });
                }
                await call.RequestStream.CompleteAsync();
                await responseTask;
            }
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var client = new GrpcClient();
            client.PostRate().Wait();
            client.SendMetadataAsync().Wait();
            client.GetByBadgeNumber().Wait();
            client.GetAll().Wait();
            try
            {
                client.AddPhoto().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
            client.SaveAll().Wait();
        }
    }
}
