module RatesPublisher.Tests.PerformanceTestUtil

open System

type GCStatus =
    { Gen0: Int32
      Gen1: Int32
      Gen2: Int32
      TotalMemory: Int64 }

let getGCStatus() =
    { Gen0 = GC.CollectionCount 0
      Gen1 = GC.CollectionCount 1
      Gen2 = GC.CollectionCount 2
      TotalMemory = GC.GetTotalMemory false  }

let time label n functionUnderTest =
    printfn "performance test: %s" label
    System.GC.Collect()
    let stopWatch = System.Diagnostics.Stopwatch()
    let gcStatusBefore = getGCStatus()
    stopWatch.Restart()
    [1..n] |> List.iter (fun _ -> functionUnderTest())
    stopWatch.Stop()
    let gcStatusAfter = getGCStatus()
    printfn "time elapsed: %6ims" stopWatch.ElapsedMilliseconds
    printfn "before: %A" gcStatusBefore
    printfn "after: %A" gcStatusAfter
    let changeInMemory = (gcStatusAfter.TotalMemory - gcStatusBefore.TotalMemory) / 1000L
    printfn "change in memory: %A K" changeInMemory