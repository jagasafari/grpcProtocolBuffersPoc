using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using static Messages.EmployeeService;

namespace GrpcServer
{
    public class EmployeeService
    {
        public async Task<EmployeeResponse> GetByBadgeNumber(
            GetByBadgeNumberRequest request, ServerCallContext context)
        {
            Metadata md = context.RequestHeaders;
            foreach (var entry in md)
                Console.WriteLine(entry.Key + ": " + entry.Value);

            foreach (var e in Employees.employees)
                if (request.BadgeNumber == e.BadgeNumber)
                    return new EmployeeResponse() { Employee = e };
            throw new Exception("Employee not found with Badge Number: " +
                request.BadgeNumber);
        }

        public async Task GetAll(GetAllRequest request, IServerStreamWriter<EmployeeResponse> responseStream, ServerCallContext context)
        {
            foreach (var e in Employees.employees)
                await responseStream.WriteAsync(new EmployeeResponse() { Employee = e });
        }

        public async Task SaveAll(
            IAsyncStreamReader<EmployeeRequest> requestStream,
            IServerStreamWriter<EmployeeResponse> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var employee = requestStream.Current.Employee;
                lock (Employees.dataLock)
                    Employees.employees.Add(employee);

                await responseStream.WriteAsync(new EmployeeResponse
                {
                    Employee = employee
                });
            }

            Console.WriteLine("Employees");
            foreach (var e in Employees.employees)
                Console.WriteLine($"{ e.BadgeNumber}; {e.Id}");
        }

        public async Task<AddPhotoResponse> AddPhoto(
          IAsyncStreamReader<AddPhotoRequest> requestStream,
          ServerCallContext context)
        {
            var data = new List<byte>();
            while (await requestStream.MoveNext())
            {
                Console.WriteLine("Received " +
                    requestStream.Current.Data.Length + " bytes");
                data.AddRange(requestStream.Current.Data);
            }
            Console.WriteLine("Received file with " + data.Count + " bytes");
            return new AddPhotoResponse() { IsOk = true };
        }

        public async Task<RateResponse> PostRate(
            RateMessageRequest request, ServerCallContext context)
        {
            Console.WriteLine($"{request.Symbol}, {request.Ask}");
            return new RateResponse();
        }
    }
    public class GrpcServer
    {
        public static ServerServiceDefinition BindService(EmployeeService serviceImpl)
   => ServerServiceDefinition.CreateBuilder()
       .AddMethod(__Method_PostRate, serviceImpl.PostRate)
       .AddMethod(__Method_GetByBadgeNumber, serviceImpl.GetByBadgeNumber)
       .AddMethod(__Method_GetAll, serviceImpl.GetAll)
       .AddMethod(__Method_SaveAll, serviceImpl.SaveAll)
       .AddMethod(__Method_AddPhoto, serviceImpl.AddPhoto)
     .Build();

        private Server _server;
        public void Start()
        {
            const int Port = 555;

            var server = new Server
            {
                Ports = { new ServerPort("0.0.0.0", Port, ServerCredentials.Insecure) },
                Services = { BindService(new EmployeeService()) }
            };
            Console.WriteLine("Starting server on port " + Port);
            server.Start();
            Console.WriteLine("Starting server on port " + Port);
            _server = server;
        }
        public void Stop()
        {
            _server.ShutdownAsync().Wait();
        }
    }
    public class Program {
        public static void Main(string[] args) {
            var ser = new GrpcServer();
            ser.Start();
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
            ser.Stop(); }
        } }
