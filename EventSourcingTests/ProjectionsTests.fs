module ProjectionsTests

open Domain
open Domain.Core
open Xunit
open FsUnit.Xunit
open Projections

[<Fact>]
let ``Project into remote scan statistics`` () =
    let remoteSipovo = Remote "Sipovo"

    let events =
        [ remoteSipovo |> RemoteWasScanned
          remoteSipovo |> RemoteWasScanned
          remoteSipovo |> RemoteWasScanned ]

    let scanStatistics = events |> project toScanStatistics

    scanStatistics.Count |> should equal 1
    scanStatistics
    |> Map.find remoteSipovo
    |> should equal 3

[<Fact>]
let ``Project into remote current state`` () =
    let remoteSipovo = Remote "Sipovo"
    let remoteBrod = Remote "Brod"
    let events =
        [ remoteSipovo |> RemoteWentOffline
          remoteSipovo |> RemoteWentOnline
          remoteBrod |> RemoteWentOffline ]

    let remoteStatus = events |> project toRemoteStatus

    remoteStatus.Count |> should equal 2
    remoteStatus
    |> Map.find remoteSipovo
    |> should equal Online
    remoteStatus
    |> Map.find remoteBrod
    |> should equal Offline
