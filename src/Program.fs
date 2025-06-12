open System

open Thuja
open Thuja.Tutu
open Thuja.Elements

open YouTubeSearch
open Scrapper
open Helpers
open Helpers.View

// model
type Model = Menu.Model<YouTubeVideoInfo>

type Msg = 
  | MenuMsg of Menu.Msg
  | Confirm

// view
let view model =
  columns [ Fraction 45; Fraction 55 ] [
    Menu.view model
    panel [] [
      rows [ Absolute 5; Absolute 1; Fraction 100 ] [
        videoInfo model.Selected
        hr []
        raw getThumbnail model.Selected
      ]
    ]
  ]

// update
let update (model : Model) : Msg -> Model * Cmd<_> = function
  | MenuMsg msg -> Menu.update model msg, Cmd.none
  | Confirm -> model, (
      Program.writeOutput model.Selected.Url // stdout
      Program.exit()
    )

// input
let keyBindings = function
  | Char 'q', _
  | Char 'c', KeyModifiers.Ctrl -> Program.exit()

  | Up, _ | Char 'k', _ -> Cmd.ofMsg (MenuMsg Menu.Previous)
  | Down, _ | Char 'j', _ -> Cmd.ofMsg (MenuMsg Menu.Next)

  | Char 'o', _
  | Enter, _ -> Cmd.ofMsg Confirm
  
  | _ ->  Cmd.none

// program
let query = 
  Environment.GetCommandLineArgs()
  |> Seq.tryItem 1
  |> Option.defaultWith ^fun () -> "Search query is empty."

let model = Menu.init (search query) _.Title

// hack: render tui in stderr to allow writing result to stdout
Console.SetOut Console.Error

Program.make model view update
|> Program.withKeyBindings keyBindings
|> Program.withTutuBackend
|> Program.run