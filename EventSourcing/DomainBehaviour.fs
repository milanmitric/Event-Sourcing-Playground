module DomainBehaviour

open Core
open DomainTypes

let private getRemoteStatus events remote =
    events
    |> project Projections.toRemoteStatus
    |> Map.tryFind remote

let onScan remote events =
    match getRemoteStatus events remote with
    | Some state -> if state = Online then [ RemoteWasAlreadyOnline remote ] else [ RemoteWentOnline remote ]
    | None -> [ RemoteWasNotFound remote ]

let offScan remote events =
    match getRemoteStatus events remote with
    | Some state -> if state = Offline then [ RemoteWasAlreadyOffline remote ] else [ RemoteWentOffline remote ]
    | None -> [ RemoteWasNotFound remote ]

let scan remote events =
    match getRemoteStatus events remote with
    | Some state -> if state = Online then [ RemoteWasScanned remote ] else [ OffLineRemoteNotScanned remote ]
    | None -> [ RemoteWasNotFound remote ]

let configure remote events =
    match getRemoteStatus events remote with
    | None -> [ RemoteSuccessfullyConfigured remote ]
    | Some _ -> [ RemoteWasAlreadyConfigured remote ]
