#r "System.Management.Automation"

open System.Management.Automation

let protocolBuffersCompiler =
    "..\\packages\\Google.Protobuf.Tools\\tools\\windows_x64\\protoc.exe"

let generateProtoClassScript message =
    sprintf
        "& %s --csharp_out=. %s.proto"
        protocolBuffersCompiler
        message

let generateProtoClasses message =
    PowerShell.Create()
        .AddScript(generateProtoClassScript message)
        .Invoke()
        |> Seq.iter (printfn "PowerShell: %O")

["rate"] |> List.iter generateProtoClasses