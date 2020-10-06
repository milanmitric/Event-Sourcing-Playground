module EventStoreTests

open Infrastructure
open Domain
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
    |> RemoteOnline
    |> List.singleton
    |> eventStore.Append


    eventStore.Get()
    |> should equal [ RemoteOnline <| Remote "Sipovo" ]

[<Fact>]
let ``Append multiple events`` () =
    let eventStore = initialize ()
    let testRemote = Remote "Sipovo"

    let events =
        [ testRemote |> RemoteOffline
          testRemote |> RemoteOnline
          testRemote |> RemoteScan ]

    eventStore.Append events

    eventStore.Get() |> should equal events
