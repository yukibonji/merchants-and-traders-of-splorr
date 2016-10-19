open Pdg.Splorr.MerchantsAndTraders.DataLayer
open System

let rec generate (generator: unit -> 'T) (sieve: 'T -> 'T -> bool) (thresholdCheck: (int * Set<'T>) -> bool) (input: int * Set<'T>) : Set<'T> =
    if input |> thresholdCheck then
        input |> snd
    else
        let candidate = generator()
        if input |> snd |> Set.forall (fun x -> sieve x candidate) then
            generate generator sieve thresholdCheck (0, input |> snd |> Set.add candidate)
        else
            generate generator sieve thresholdCheck (1 + (input |> fst), input |> snd)

let positionGenerator (random:Random) (width:float) (height:float) : Position<float> =
    { X = random.NextDouble() * width;
      Y = random.NextDouble() * height }

let sieve (minimumDistance:float) (existing: Position<float>) (candidate: Position<float>) : bool =
    let deltaX = candidate.X - existing.X
    let deltaY = candidate.Y - existing.Y
    let distance2 = deltaX * deltaX + deltaY * deltaY
    minimumDistance * minimumDistance <= distance2

let thresholdCheck (maximumTries:int) (count:int, current:Set<'T>) : bool =
    count >= maximumTries
    

[<EntryPoint>]
let main argv = 
    let random = Random()
    let positions =
        generate (fun () -> positionGenerator random 500.0 500.0) (sieve 10.0) (thresholdCheck 5000) (0,Set.empty)
    0
