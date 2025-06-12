module YouTubeSearch.Helpers

module View =
  open Thuja.Styles
  open Thuja.Elements
  
  let videoInfo item = 
    let info label content =
      region [] [
        text [] $"{label}: "
        region [ Margin (Left $"{label}: ".Length) ] [ 
          text [ Attributes [ Attribute.Dim ]; Overflow Ellipsis ] content 
        ]
      ]

    rows [ Absolute 1; Absolute 1; Absolute 1; Absolute 1; Absolute 1 ] [
      info (nameof item.Title) item.Title
      info (nameof item.Channel) item.Channel
      info (nameof item.Duration) item.Duration
      info (nameof item.Views) item.Views
      info (nameof item.Date) item.Date
    ]

module Program =
  open System
  open System.IO
  
  let writeOutput (result : string) =
    use writer = new StreamWriter(Console.OpenStandardOutput())
    writer.Write result