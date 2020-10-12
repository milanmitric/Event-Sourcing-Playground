module Infrastructure

open Domain.Core

type private Msg<'Event> =
    | Append of 'Event list
    | Get of AsyncReplyChannel<'Event list>
    | Evolve of EventProducer<'Event>

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
                    | Evolve eventProducer ->
                        let newEvents = eventProducer history
                        return! innerLoop (history @ newEvents)
                }

            innerLoop [])

    let get () = mailbox.PostAndReply Get
    let append events = events |> Append |> mailbox.Post
    let evolve eventProducer = mailbox.Post(Evolve eventProducer)

    { Get = get
      Append = append
      Evolve = evolve }
