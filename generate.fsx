#r "System.Management.Automation"

open System.Management.Automation

PowerShell.Create()
    .AddScript(
        "& '.\\packages\\Google.Protobuf.Tools\\tools\\windows_x64\\protoc.exe' --csharp_out=.\ProtoClasses .\ProtoClasses\customer.proto")
    .Invoke()
    |> Seq.iter (printfn "PowerShell: %O")