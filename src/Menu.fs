[<RequireQualifiedAccess>]
module YouTubeSearch.Menu 

open System

open Thuja
open Thuja.Styles
open Thuja.Elements

// model
type Model<'item> private =
  { Items: 'item list 
    Index: int
    Selector: 'item -> string }
  with
    member this.Selected with get() = 
      this.Items.[this.Index]

type Msg =
  | Next
  | Previous

let init items selector =
  { Items = items
    Index = 0
    Selector = selector }

// view
let view (model : Model<_>) = 
  let status (region : View.Region) = 
    let line = String.replicate region.Width "─"
    let bar = $"{model.Index + 1}/{model.Items.Length}{line}"
    text [ Color Color.DarkYellow ] bar region

  let items = 
    model.Items
    |> Seq.map model.Selector
    |> Seq.toList

  rows [ Absolute 1; Fraction 100 ] [
    status
    list items model.Index
  ]
  
// update
let update model = function
  | Next -> { model with Index = Math.Min(model.Items.Length - 1, model.Index + 1) }
  | Previous -> { model with Index = Math.Max(0, model.Index - 1) }