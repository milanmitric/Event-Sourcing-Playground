module Domain

type Remote = Remote of string

type Event =
    | RemoteOnline of Remote
    | RemoteOffline of Remote
    | RemoteScan of Remote
