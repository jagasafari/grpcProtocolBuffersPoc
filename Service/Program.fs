module Service.Program

open Topshelf
open System
open Service.Util

let setupUnhandledExceptionsHandling() =
    AppDomain.CurrentDomain.UnhandledException
    |> Event.add (fun args ->
        sprintf "Fatal unhandled error %A" ((args.ExceptionObject) :?> Exception) |> print)

[<EntryPoint>]
let main _ =
    printfn "%s" "starting protocol buffers grpc poc"
    setupUnhandledExceptionsHandling()
    HostFactory.Run(fun host ->
        host.Service<TradeAirService>(TradeAirServiceConfigurator.Init) |> ignore
        let serviceName = "TradeAir.Service"
        serviceName |> host.SetDisplayName
        serviceName |> host.SetServiceName
        host.StartAutomatically() |> ignore
    ) |> ignore
    0