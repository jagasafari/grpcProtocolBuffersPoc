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
    public class Program
    {
        const int Port = 555;
        public static void Main(string[] args)
        {
            var channel = new Channel("localhost", Port, ChannelCredentials.Insecure);

            PostRate(channel).Wait();
            SendMetadataAsync(channel).Wait();
            GetByBadgeNumber(channel).Wait();
            GetAll(channel).Wait();
            try
            {
                AddPhoto(channel).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
            SaveAll(channel).Wait();
        }

        private static async Task PostRate(Channel channel)
        {
            var res = await new DefaultCallInvoker(channel)
                    .AsyncUnaryCall(__Method_PostRate, null,
                        new CallOptions(),
                        new RateMessageRequest() {
                            Symbol = "abc",
                            Ask =20m});
            Console.WriteLine(res);
        }

        public static async Task SendMetadataAsync(Channel channel)
        {
            Metadata md = new Metadata();
            md.Add("username", "mvansickle");
            md.Add("password", "password1");
            try
            {
                await new DefaultCallInvoker(channel)
                    .AsyncUnaryCall(__Method_GetByBadgeNumber, null,
                        new CallOptions().WithHeaders(md),
                        new GetByBadgeNumberRequest());
            }
            catch (Exception e) {
            }
            Console.WriteLine("\n\n Metadata");
        }
        public static async Task GetByBadgeNumber(Channel channel)
        {
            var res = await new DefaultCallInvoker(channel)
                    .AsyncUnaryCall(__Method_GetByBadgeNumber, null,
                        new CallOptions(),
                        new GetByBadgeNumberRequest() { BadgeNumber = 2080 });
            Console.WriteLine($"\n\ngetbybadge number {res.Employee}");
        }

        public static async Task GetAll(Channel channel)
        {
            using (var call =
                new DefaultCallInvoker(channel).AsyncServerStreamingCall(__Method_GetAll, null, new CallOptions(),
                    new GetAllRequest()))
            {
                var responseStream = call.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    Console.WriteLine($"getting: {responseStream.Current.Employee.LastName}");
                }
            }
        }

        public static async Task AddPhoto(Channel channel)
        {
            Metadata md = new Metadata();
            md.Add("badgenumber", "2080");

            FileStream fs = File.OpenRead("Penguins.jpg");
            using (var call = new DefaultCallInvoker(channel)
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

        private static async Task SaveAll(Channel channel)
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
            using (var call = new DefaultCallInvoker(channel)
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
}
