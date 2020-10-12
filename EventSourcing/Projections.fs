module Projections

open Domain
open Infrastructure

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
