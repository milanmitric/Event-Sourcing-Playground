module Domain

type RemoteStatus =
    | Online
    | Offline

type Remote = Remote of string

type Event =
    | RemoteWentOnline of Remote
    | RemoteWentOffline of Remote
    | RemoteWasScanned of Remote

type EventStore<'Event> =
    { Get: unit -> 'Event list
      Append: 'Event list -> unit }

type Projection<'State, 'Event> =
    { Init: 'State
      Update: 'State -> 'Event -> 'State }

let project (projection: Projection<_, _>) events =
    events
    |> List.fold projection.Update projection.Init


module Projections =

    let toRemoteStatus: Projection<Map<Remote, RemoteStatus>, Event> =
        let updateRemoteStatus state event =
            match event with
            | RemoteWentOnline remote -> state |> Map.add remote Online
            | RemoteWentOffline remote -> state |> Map.add remote Offline
            | _ -> state

        { Init = Map.empty
          Update = updateRemoteStatus }

    let toScanStatistics: Projection<Map<Remote, int>, Event> =
        let countScanStatistics state event =
            match event with
            | RemoteWasScanned remote ->
                let scanCount =
                    state
                    |> Map.tryFind remote
                    |> Option.defaultValue 0

                Map.add remote (scanCount + 1) state
            | _ -> state

        { Init = Map.empty
          Update = countScanStatistics }
