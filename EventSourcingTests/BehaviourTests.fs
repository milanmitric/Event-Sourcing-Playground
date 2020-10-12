module BehaviourTests

open Domain
open FsUnit
open TestData
open Xunit
open TestInfrastructure

[<Fact>]
let ``OnScan a offline remote`` () =
    Given [RemoteWentOffline sipovoRemote]
    |> When (onScan sipovoRemote)
    |> Then [RemoteWentOnline sipovoRemote]
   
[<Fact>]
let ``OnScan an already online remote`` () =
    Given [RemoteWentOnline sipovoRemote]
    |> When (onScan sipovoRemote)
    |> Then [RemoteWasAlreadyOnline sipovoRemote]
    
[<Fact>]
let ``Cant onScan unknown remote`` () =
    Given []
    |> When (onScan sipovoRemote)
    |> Then [RemoteWasNotFound sipovoRemote]