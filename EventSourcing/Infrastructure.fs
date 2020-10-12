module Infrastructure

open Domain

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
