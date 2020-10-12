module Domain

type RemoteStatus = 
    | Online
    | Offline

type Remote = Remote of string

type Event =
    | RemoteWentOnline of Remote
    | RemoteWentOffline of Remote
    | RemoteWasScanned of Remote
