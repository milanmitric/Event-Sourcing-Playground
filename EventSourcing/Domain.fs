module Domain

type RemoteStatus =
    | Online
    | Offline

type Remote = Remote of string

type Event =
    | RemoteWentOnline of Remote
    | RemoteWasAlreadyOnline of Remote
    | RemoteWentOffline of Remote
    | RemoteWasAlreadyOffline of Remote
    | RemoteWasScanned of Remote
    | OffLineRemoteNotScanned of Remote
    | RemoteWasNotFound of Remote
    | RemoteSuccessfullyConfigured of Remote
    | RemoteWasAlreadyConfigured of Remote

module Core =

    type Aggregate = System.Guid

    type EventProducer<'Event> = 'Event list -> 'Event list

    type EventStore<'Event> =
        { Get: unit -> Map<Aggregate, 'Event list>
          GetStream: Aggregate -> 'Event list
          Append: Aggregate -> 'Event list -> unit
          Evolve: Aggregate -> EventProducer<'Event> -> unit }

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
            | (RemoteSuccessfullyConfigured remote )| (RemoteWentOffline remote)-> state |> Map.add remote Offline
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
    let private getRemoteStatus events remote =
        events 
        |> Core.project Projections.toRemoteStatus
        |> Map.tryFind remote

    let onScan remote events =
        match getRemoteStatus events remote with
        | Some state -> if state = Online then [ RemoteWasAlreadyOnline remote ] else [ RemoteWentOnline remote ]
        | None -> [ RemoteWasNotFound remote ]

    let offScan remote events =
        match getRemoteStatus events remote with
        | Some state -> if state = Offline then [RemoteWasAlreadyOffline remote] else [RemoteWentOffline remote]
        | None -> [RemoteWasNotFound remote]

    let scan remote events = 
        match getRemoteStatus events remote with
        | Some state -> if state = Online then [RemoteWasScanned remote] else [OffLineRemoteNotScanned remote]
        | None -> [RemoteWasNotFound remote]

    let configure remote events =
        match getRemoteStatus events remote with
        | None -> [RemoteSuccessfullyConfigured remote]
        | Some _ -> [RemoteWasAlreadyConfigured remote]