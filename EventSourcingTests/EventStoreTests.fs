module EventStoreTests

open Infrastructure
open Domain
open Domain.Core
open Xunit
open FsUnit.Xunit


[<Fact>]
let ``Initialized event store is empty`` () =
    let eventStore = initialize ()
    
    eventStore.Get() |> should be Empty

[<Fact>]
let ``Append single event`` () =
    let eventStore = initialize ()

    Remote "Sipovo"
    |> RemoteWentOnline
    |> List.singleton
    |> eventStore.Append

    eventStore.Get()
    |> should equal [ RemoteWentOnline <| Remote "Sipovo" ]

[<Fact>]
let ``Append multiple events`` () =
    let eventStore = initialize ()
    let testRemote = Remote "Sipovo"
    let events =
        [ testRemote |> RemoteWentOffline
          testRemote |> RemoteWentOnline
          testRemote |> RemoteWasScanned ]

    eventStore.Append events

    eventStore.Get() |> should equal events
