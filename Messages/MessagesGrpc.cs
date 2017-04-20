using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Messages;

namespace Messages {
  public static class EmployeeService
  {
    public static readonly Method<RateMessageRequest, RateResponse> __Method_PostRate =
        GrpcUtil.CreateMethod<RateMessageRequest, RateResponse>(
        MethodType.Unary, "PostRate");

    public static readonly Method<GetByBadgeNumberRequest, EmployeeResponse> __Method_GetByBadgeNumber =
        GrpcUtil.CreateMethod<GetByBadgeNumberRequest, EmployeeResponse>(
        MethodType.Unary, "GetByBadgeNumber");

    public static readonly Method<GetAllRequest, EmployeeResponse> __Method_GetAll =
        GrpcUtil.CreateMethod<GetAllRequest, EmployeeResponse>(
        MethodType.ServerStreaming, "GetAll");

    public static readonly Method<EmployeeRequest, EmployeeResponse> __Method_SaveAll =
        GrpcUtil.CreateMethod<EmployeeRequest, EmployeeResponse>(
        MethodType.DuplexStreaming, "SaveAll");

    public static readonly Method<AddPhotoRequest, AddPhotoResponse> __Method_AddPhoto =
        GrpcUtil.CreateMethod<AddPhotoRequest, AddPhotoResponse>(
        MethodType.ClientStreaming, "AddPhoto");
    }
}