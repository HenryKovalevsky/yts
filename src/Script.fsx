#r "nuget: Newtonsoft.Json"

open System
open System.Net.Http
open System.Text.RegularExpressions
open Newtonsoft.Json.Linq

type YouTubeVideoResult = 
  { Url: string
    Title: string
    Channel: string
    Duration: string
    Views: string
    Date: string
    Description: string
    Id: string
    Thumbnail: string }

let parseYouTubeData json =
  let root = JToken.Parse json
  [ for vr in root.SelectTokens "$..videoRenderer" do
      if not <| isNull vr && vr.Type = JTokenType.Object then
        let getValue path = 
            let token = vr.SelectToken path
            if isNull token then String.Empty else token.ToString()
        
        let videoId = getValue "videoId"
        { Id = videoId
          Url = $"https://www.youtube.com/watch?v={videoId}"
          Title = getValue "title.runs[0].text"
          Channel = getValue "longBylineText.runs[0].text"
          Duration = getValue "lengthText.simpleText"
          Views = getValue "shortViewCountText.simpleText"
          Date = getValue "publishedTimeText.simpleText"
          Description = getValue "detailedMetadataSnippets[0].snippetText.runs[0].text"
          Thumbnail = getValue "thumbnail.thumbnails[0].url" } ]

let httpClient = new HttpClient()
let search query = task {
  let! html = httpClient.GetStringAsync $"https://www.youtube.com/results?search_query={query}"
  
  let regex = Regex(@"var ytInitialData\s*=\s*({[\s\S]*?});\s*</script", RegexOptions.Multiline)

  let json = 
    regex.Match(html).Groups
    |> Seq.tryItem 1
    |> Option.map _.Value
    |> Option.defaultWith (fun () -> failwith "Couldn't load YouTube page data.")

  return parseYouTubeData json
}


#r "nuget: CliWrap"
#r "nuget: Thuja.Tutu"

open System
open System.IO
open System.Net.Http
open System.Diagnostics

open CliWrap
open CliWrap.Buffered

open Thuja
open Thuja.Tutu
open Thuja.Styles
open Thuja.Elements

[<RequireQualifiedAccess>]
module Menu = 
  type Model private =
    { Items: string list 
      Index: int }

  type Model with
    member this.Selected with get() = this.Items.[this.Index]

  let init items = 
    { Items = items
      Index = 0 }
    
  type Msg =
    | Next
    | Previous

  let update (model : Model) = function
    | Next -> { model with Index = Math.Min(model.Items.Length - 1, model.Index + 1) }
    | Previous -> { model with Index = Math.Max(0, model.Index - 1) }

  let view (model : Model) = list model.Items model.Index

// https://hpjansson.org/chafa/
let chafa (url : string) (width, height) = 
  task {
    let! stream = httpClient.GetStreamAsync url
      
    let! result =
      Cli.Wrap("chafa")
        .WithArguments(["-f"; "sixels"; "-s"; $"{width}x{height}"; "--font-ratio"; "1/1"])
        .WithStandardInputPipe(PipeSource.FromStream stream)
        .ExecuteBufferedAsync()

    return result.StandardOutput
  }
  |> Async.AwaitTask
  |> Async.RunSynchronously

// model
type Model =
  { Results: YouTubeVideoResult list
    Menu: Menu.Model }
  with 
    member this.Selected = Seq.item this.Menu.Index this.Results
    static member init results =
      let items = 
        results 
        |> Seq.map (fun i -> i.Title) 
        |> Seq.toList
      { Results = results
        Menu = Menu.init items }

type Msg = 
  | MenuMsg of Menu.Msg
  | Run
  | Exit

let results =
  search "miles davis"
  |> Async.AwaitTask
  |> Async.RunSynchronously

let model = Model.init results

let hr (region : View.Region) = 
  text [] (String.replicate region.Width "─") region
let status (menu : Menu.Model) (region : View.Region) = 
  text [ Color Color.DarkYellow ] ($"{menu.Index + 1}/{menu.Items.Length}" + String.replicate region.Width "─") region

// view
let view model =
  let info label content =
    let label = $"{label}: "
    region [] [
      text [] label
      region [ Margin (Left label.Length) ] [ text [ Color Color.DarkGrey; Overflow Ellipsis ] content ]
    ]

  columns [ Fraction 45; Fraction 55 ] [
    rows [ Absolute 1; Fraction 100 ] [
      status model.Menu
      Menu.view model.Menu
    ]
    panel [] [
      rows [ Absolute 5; Absolute 1; Fraction 100 ] [
        rows [ Absolute 1; Absolute 1; Absolute 1; Absolute 1; Absolute 1 ] [
          info (nameof model.Selected.Title) model.Selected.Title
          info (nameof model.Selected.Channel) model.Selected.Channel
          info (nameof model.Selected.Duration) model.Selected.Duration
          info (nameof model.Selected.Views) model.Selected.Views
          info (nameof model.Selected.Date) model.Selected.Date
        ]
        hr
        raw chafa model.Selected.Thumbnail
      ]
    ]
  ]

let mutable result = ""

// update
let update (model : Model) : Msg -> Model * Cmd<_> = function
  | MenuMsg msg -> { model with Menu = Menu.update model.Menu msg }, Cmd.none
  | Run -> model, Cmd.ofFunc (fun () -> 
      result <- model.Selected.Url
      Exit
    )
  | Exit -> model, Program.exit()

// input
let keyBindings = function
  | Char 'q', _
  | Char 'c', KeyModifiers.Ctrl -> Cmd.ofMsg Exit

  | Up, _ | Char 'k', _ -> Cmd.ofMsg (MenuMsg Menu.Previous)
  | Down, _ | Char 'j', _ -> Cmd.ofMsg (MenuMsg Menu.Next)

  | Enter, _ -> Cmd.ofMsg Run
  
  | _ ->  Cmd.none


// program
Program.make model view update
|> Program.withKeyBindings keyBindings
|> Program.withTutuBackend
|> Program.run
