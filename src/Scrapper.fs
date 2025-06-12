namespace YouTubeSearch

open System
open System.Net.Http
open System.Text.RegularExpressions

open CliWrap
open CliWrap.Buffered
open Newtonsoft.Json.Linq

type YouTubeVideoInfo = 
  { Url: string
    Title: string
    Channel: string
    Duration: string
    Views: string
    Date: string
    Description: string
    Id: string
    Thumbnail: string }

module Scrapper =
  let private httpClient = new HttpClient()

  let private parseYouTubeData json =
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

  let search query = 
    task {
      let! html = httpClient.GetStringAsync $"https://www.youtube.com/results?search_query={query}"
      
      let regex = Regex(@"var ytInitialData\s*=\s*({[\s\S]*?});\s*</script", RegexOptions.Multiline)

      let json = 
        html
        |> regex.Match
        |> _.Groups
        |> Seq.tryItem 1
        |> Option.map _.Value
        |> Option.defaultWith ^fun () -> failwith "Couldn't load YouTube page data."

      return parseYouTubeData json
    } 
    |> Async.AwaitTask
    |> Async.RunSynchronously

  let getThumbnail (data : YouTubeVideoInfo) (width, height) = 
    task {
      let! stream = httpClient.GetStreamAsync data.Thumbnail
        
      // https://hpjansson.org/chafa/
      let! result =
        Cli.Wrap "chafa"
        |> _.WithArguments(["-f"; "sixels"; "-s"; $"{width}x{height}"; "--font-ratio"; "1/1"])
        |> _.WithStandardInputPipe(PipeSource.FromStream stream)
        |> _.ExecuteBufferedAsync()

      return result.StandardOutput
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously


