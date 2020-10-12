module Projections

open Domain

type Projection<'State, 'Event> = 
    {
        Init : 'State
        Update : 'State -> 'Event -> 'State
    }

let project (projection: Projection<_,_> ) events =
    events |> List.fold projection.Update projection.Init

let countScanStatistics state event =
    match event with
    | RemoteWasScaned remote ->
        let scanCount = 
            state 
            |> Map.tryFind remote
            |> Option.defaultValue 0
        
        Map.add remote (scanCount + 1) state
    | _ -> state

let scanStatistics : Projection<Map<Remote, int>, Event> =
    {
       Init = Map.empty
       Update = countScanStatistics
    }