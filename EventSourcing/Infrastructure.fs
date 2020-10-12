module Infrastructure

open Domain.Core

type private Msg<'Event> =
    | Append of Aggregate * 'Event list
    | Get of AsyncReplyChannel<Map<Aggregate, 'Event list>>
    | GetStream of Aggregate * AsyncReplyChannel<'Event list>
    | Evolve of Aggregate * EventProducer<'Event>

let private streamEventsFrom history aggregate =
    history
    |> Map.tryFind aggregate
    |> Option.defaultValue []

let initialize (): EventStore<'Event> =

    let mailbox =
        MailboxProcessor.Start(fun inbox ->
            let rec innerLoop history =
                let getEventsFor = streamEventsFrom history

                async {
                    match! inbox.Receive() with
                    | Append (aggregate, newEvents) ->
                        let newHistory =
                            history
                            |> Map.add aggregate (getEventsFor aggregate @ newEvents)

                        return! innerLoop newHistory
                    | Get channel ->
                        channel.Reply history
                        return! innerLoop history
                    | GetStream (aggregate, channel) ->
                        channel.Reply <| getEventsFor aggregate
                        return! innerLoop history
                    | Evolve (aggregate, eventProducer) ->
                        let streamEvents = getEventsFor aggregate
                        let newEvents = eventProducer streamEvents

                        let newHistory =
                            history
                            |> Map.add aggregate (streamEvents @ newEvents)

                        return! innerLoop newHistory
                }

            innerLoop Map.empty)

    let get () = mailbox.PostAndReply Get

    let getStream aggregate =
        mailbox.PostAndReply(fun channel -> GetStream(aggregate, channel))

    let append aggregate events =
        Append(aggregate, events) |> mailbox.Post

    let evolve aggregate eventProducer =
        Evolve(aggregate, eventProducer) |> mailbox.Post

    { Get = get
      Append = append
      Evolve = evolve
      GetStream = getStream }
