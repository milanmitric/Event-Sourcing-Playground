module TestInfrastructure

open FsUnit.Xunit

let Given = id

let When eventProducer events = eventProducer events

let Then actualEvents expectedEvents =
    expectedEvents |> should equal actualEvents
