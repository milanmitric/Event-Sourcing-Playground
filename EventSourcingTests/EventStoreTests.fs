module EventStoreTests

open Infrastructure
open Domain
open Domain.Core
open Xunit
open FsUnit.Xunit
open TestData

[<Fact>]
let ``Initialized event store is empty`` () =
    let eventStore = initialize ()

    eventStore.Get() |> should be Empty

[<Fact>]
let ``Append single event`` () =
    let eventStore = initialize ()

    sipovoRemote
    |> RemoteWentOnline
    |> List.singleton
    |> eventStore.Append

    eventStore.Get()
    |> should equal [ RemoteWentOnline sipovoRemote ]

[<Fact>]
let ``Append multiple events`` () =
    let eventStore = initialize ()

    let events =
        [ sipovoRemote |> RemoteWentOffline
          sipovoRemote |> RemoteWentOnline
          sipovoRemote |> RemoteWasScanned ]

    eventStore.Append events

    eventStore.Get() |> should equal events
