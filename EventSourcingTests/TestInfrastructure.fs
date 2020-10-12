module TestInfrastructure

open FsUnit.Xunit

let Given = id

let When eventProducer events = eventProducer events

let Then expectedEvents actualEvents =
    expectedEvents |> should equal actualEvents
