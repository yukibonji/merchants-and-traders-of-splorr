﻿open Pdg.Splorr.MerchantsAndTraders.DataLayer
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

let positionSieve (minimumDistance:float) (existing: Position<float>) (candidate: Position<float>) : bool =
    let deltaX = candidate.X - existing.X
    let deltaY = candidate.Y - existing.Y
    let distance2 = deltaX * deltaX + deltaY * deltaY
    minimumDistance * minimumDistance <= distance2

let positionThresholdCheck (maximumTries:int) (count:int, current:Set<'T>) : bool =
    count >= maximumTries

let nameGenerator (random:Random) (lengthGenerator:Random->int) (vowelGenerator:Random->bool) (vowels:seq<string>) (consonants:seq<string>) : string =
    let nameLength = lengthGenerator random
    let vowel = vowelGenerator random
    (vowel,nameLength)
    |> Seq.unfold (
        fun (v,l)->
            if l=0 then
                None
            else
                let letters = if v then vowels else consonants
                let letter = letters |> Seq.item (random.Next(letters |> Seq.length))
                (letter, (v |> not, l - 1))
                |> Some
        )
    |> Seq.reduce (+)

let nameThresholdCheck (numberRequired:int) (count:int, current:Set<'T>) :bool =
    current.Count >= numberRequired


[<EntryPoint>]
let main argv = 
    let random = Random()
    let positions =
        generate (fun () -> positionGenerator random 250.0 250.0) (positionSieve 10.0) (positionThresholdCheck 5000) (0,Set.empty)
        |> Set.toList
    let randomName () = nameGenerator random (fun r->r.Next(4)+r.Next(4)+r.Next(4)+3) (fun r->r.Next(2)=1) ["a";"e";"i";"o";"u"] ["h";"k";"l";"m";"p"]
    let names = 
        generate 
            (randomName)
            (fun x->(<>) x)
            (nameThresholdCheck (positions|> Seq.length))
            (0, Set.empty)
        |> Set.toList
        |> List.sortBy (fun x->random.Next())




    let context = Context.create()

    let world = 
        context
        |> WorldRepository.create {WorldId =0; WorldName=randomName();CreatedOn=DateTimeOffset.MinValue}

    let sites =
        List.zip names positions
        |> List.map (fun (n,p) ->{Site.SiteId = 0; WorldId = world.WorldId; SiteName = n; Position = p})
        |> List.map (fun s -> SiteRepository.create s context)

    0