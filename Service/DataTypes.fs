namespace Service

open Topshelf
open Service.Util

type TradeAirService() =
    member __.OnStart() = print "starting"
    member __.OnStop() = print "stopping"

type TradeAirServiceConfigurator =
    static member Init(sc:ServiceConfigurators.ServiceConfigurator<TradeAirService>) =
        sc.ConstructUsing(fun (_: string) -> new TradeAirService()) |> ignore
        sc.WhenStarted(fun (s : TradeAirService) -> s.OnStart()) |> ignore
        sc.WhenStopped(fun (s : TradeAirService) -> s.OnStop()) |> ignore