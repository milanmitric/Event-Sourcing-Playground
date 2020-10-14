module RemoteSpecifications

open Domain
open Domain.Behaviour
open FsUnit
open TestData
open Xunit
open TestInfrastructure

[<Fact>]
let ``OnScan a offline remote`` () =
    Given [RemoteWentOffline sipovoRemote]
    |> When onScan sipovoRemote
    |> Then [RemoteWentOnline sipovoRemote]
   
[<Fact>]
let ``OnScan an already online remote`` () =
    Given [RemoteWentOnline sipovoRemote]
    |> When onScan sipovoRemote
    |> Then [RemoteWasAlreadyOnline sipovoRemote]
    
[<Fact>]
let ``Cant onScan unknown remote`` () =
    Given []
    |> When onScan sipovoRemote
    |> Then [RemoteWasNotFound sipovoRemote]

[<Fact>]
let ``OffScan a online remote`` () =
    Given [RemoteWentOnline sipovoRemote]
    |> When offScan sipovoRemote
    |> Then [RemoteWentOffline sipovoRemote]
   
[<Fact>]
let ``OffScan an already offline remote`` () =
    Given [RemoteWentOffline sipovoRemote]
    |> When offScan sipovoRemote
    |> Then [RemoteWasAlreadyOffline sipovoRemote]
    
[<Fact>]
let ``Cant offScan unknown remote`` () =
    Given []
    |> When offScan sipovoRemote
    |> Then [RemoteWasNotFound sipovoRemote]

[<Fact>]
let ``Cant scan offline remote`` () =
    Given [RemoteWentOffline sipovoRemote]
    |> When scan sipovoRemote
    |> Then [OffLineRemoteNotScanned sipovoRemote]

[<Fact>]
let ``Cant scan uknown remote`` () =
    Given []
    |> When scan sipovoRemote
    |> Then [RemoteWasNotFound sipovoRemote]

[<Fact>]
let ``Scan and online remote`` () =
    Given [RemoteWentOnline sipovoRemote]
    |> When scan sipovoRemote
    |> Then [RemoteWasScanned sipovoRemote]

[<Fact>]
let ``Configure remote``() =
    Given [] 
    |> When configure sipovoRemote
    |> Then [RemoteSuccessfullyConfigured sipovoRemote]

[<Fact>]
let ``Try to configure existing remote``() =
    Given [RemoteSuccessfullyConfigured sipovoRemote]
    |> When configure sipovoRemote
    |> Then [RemoteWasAlreadyConfigured sipovoRemote]

