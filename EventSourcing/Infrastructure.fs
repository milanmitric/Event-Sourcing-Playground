module Infrastructure

type EventStore<'Event> =
    { Get: unit -> 'Event list
      Append: 'Event list -> unit }

type private Msg<'Event> =
    | Append of 'Event list
    | Get of AsyncReplyChannel<'Event list>

let initialize (): EventStore<'Event> =
    let mailbox =
        MailboxProcessor.Start(fun inbox ->
            let rec innerLoop history =
                async {
                    match! inbox.Receive() with
                    | Append events -> return! innerLoop (history @ events)
                    | Get channel ->
                        channel.Reply history
                        return! innerLoop history
                }

            innerLoop [])

    let get () = mailbox.PostAndReply Get

    let append events = events |> Append |> mailbox.Post

    { Get = get; Append = append }

type Projection<'State, 'Event> =
    { Init: 'State
      Update: 'State -> 'Event -> 'State }

let project (projection: Projection<_, _>) events =
    events
    |> List.fold projection.Update projection.Init
