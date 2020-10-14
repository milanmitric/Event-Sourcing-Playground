module Utils

open Core


let printRemoteEvents remote events: unit=
    printf "Remote: %A" remote

    events 
    |> List.map (fun event -> event.ToString())
    |> List.reduce (+)
    |> printfn "%A"

let printEventStore (eventStore: EventStore<'Event>) =
    let events = eventStore.Get()

    events
    |> Map.iter printRemoteEvents

