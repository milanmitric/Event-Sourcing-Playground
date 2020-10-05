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
            let rec loop history =
                async {
                    match! inbox.Receive() with
                    | Append events -> return! loop (history @ events)
                    | Get channel ->
                        channel.Reply history
                        return! loop history
                }

            loop [])


    let get () = mailbox.PostAndReply Get

    let append events = mailbox.Post(Append events)

    { Get = get; Append = append }
