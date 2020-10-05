module EventStoreTests

open Infrastructure
open Domain
open Xunit
open FsUnit.Xunit
open Utils

[<Fact>]
let ``Initialized event store is empty`` () =
    let eventStore: EventStore<Event> = initialize ()
    eventStore.Get() |> should be Empty
