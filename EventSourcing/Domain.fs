module Domain

type RemoteStatus =
    | Online
    | Offline

type Remote = Remote of string

type Event =
    | RemoteWentOnline of Remote
    | RemoteWasAlreadyOnline of Remote
    | RemoteWentOffline of Remote
    | RemoteWasScanned of Remote
    | RemoteWasNotFound of Remote

module Core =

    type EventProducer<'Event> = 'Event list -> 'Event list

    type EventStore<'Event> =
        { Get: unit -> 'Event list
          Append: 'Event list -> unit
          Evolve: EventProducer<'Event> -> unit }

    type Projection<'State, 'Event> =
        { Init: 'State
          Update: 'State -> 'Event -> 'State }

    let project (projection: Projection<_, _>) events =
        events
        |> List.fold projection.Update projection.Init


module Projections =
    open Core

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

module Behaviour =
    let onScan remote events =
        let currentState =
            events |> Core.project Projections.toRemoteStatus

        match currentState |> Map.tryFind remote with
        | Some state -> if state = Online then [ RemoteWasAlreadyOnline remote ] else [ RemoteWentOnline remote ]
        | None -> [ RemoteWasNotFound remote ]
