module TestInfrastructure

open Domain
open Domain.Core
open FsUnit.Xunit

let Given events = events

let When eventProducer events = 
    eventProducer events

let Then expectedEvents actualEvents =
    expectedEvents 
    |> should equal actualEvents
