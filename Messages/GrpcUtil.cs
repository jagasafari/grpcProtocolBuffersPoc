using Grpc.Core;
using Messages;

namespace Messages
{
    public static class GrpcUtil
    {
        public static readonly string __ServiceName = "EmployeeService";
        private static Marshaller<T> Create<T> () =>
            Marshallers.Create(Serializer<T>.ToBytes, Serializer<T>.FromBytes);
        public static Method<T1,T2> CreateMethod<T1, T2>(MethodType methodType, string name)
                => new Method<T1, T2> (
                     methodType, __ServiceName, name, Create<T1>(), Create<T2>());
    }
}
