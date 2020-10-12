module ProjectionsTests

open Domain
open Domain.Core
open Xunit
open FsUnit.Xunit
open Projections
open TestData

[<Fact>]
let ``Project into remote scan statistics`` () =
    let events =
        [ sipovoRemote |> RemoteWasScanned
          sipovoRemote |> RemoteWasScanned
          sipovoRemote |> RemoteWasScanned ]

    let scanStatistics = events |> project toScanStatistics

    scanStatistics.Count |> should equal 1
    scanStatistics
    |> Map.find sipovoRemote
    |> should equal 3

[<Fact>]
let ``Project into remote current state`` () =
    let events =
        [ sipovoRemote |> RemoteWentOffline
          sipovoRemote |> RemoteWentOnline
          brodRemote |> RemoteWentOffline ]

    let remoteStatus = events |> project toRemoteStatus

    remoteStatus.Count |> should equal 2
    remoteStatus
    |> Map.find sipovoRemote
    |> should equal Online
    remoteStatus
    |> Map.find brodRemote
    |> should equal Offline
