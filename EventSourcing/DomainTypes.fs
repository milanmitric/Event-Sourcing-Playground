module DomainTypes

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






