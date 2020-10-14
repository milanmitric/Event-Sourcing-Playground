module IntegrationTests

open Xunit
open FsUnit.Xunit

open Core
open DomainTypes
open DomainBehaviour
open Infrastructure

open TestData

[<Fact>]
let ``Successfully scan online remote`` () =
    let eventStore: EventStore<Event> = initialize ()
    let evolveEvents = eventStore.Evolve sipovoAggregate

    evolveEvents (configure sipovoRemote)
    evolveEvents (onScan sipovoRemote)
    evolveEvents (scan sipovoRemote)

    eventStore.GetStream sipovoAggregate
    |> should
        equal
           [ RemoteSuccessfullyConfigured sipovoRemote
             RemoteWentOnline sipovoRemote
             RemoteWasScanned sipovoRemote ]
