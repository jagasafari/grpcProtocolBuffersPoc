module RatesPublisher.Tests.SerializeDesirializePoc

open Google.Protobuf
open NUnit.Framework
open System.IO
open Messages
open PerformanceTestUtil

let createRate() = Rate(RateId = 6, PairsShortName = "EUR\USD")

[<TestCase(10000)>]
let ``proto buffers: serialize, then deserilize back rate n times`` n =
    let functionUnderTest () =
        use stream = new MemoryStream()
        use codedOutputStream = new CodedOutputStream(stream)
        (createRate()).WriteTo(codedOutputStream)

    time
        (sprintf "proto buffers serialized %i times" n)
        n
        functionUnderTest