module Tests

open FsUnit.Xunit
open Xunit


[<Fact>]
let ``My test`` () =
    let result = true
    result |> should equal true
