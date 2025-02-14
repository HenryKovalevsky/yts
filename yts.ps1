param(
  [Parameter(Mandatory, HelpMessage="Search query")]
  [string]$search_query,
  
  [Parameter(HelpMessage="Audio only")]
  [switch]$a = $False
)

$url = [URI]::EscapeUriString("https://www.youtube.com/results?search_query=$($search_query)")
$mpv_cmd = if ($a.IsPresent) { "mpv -v --volume=50 --no-video" } else { "mpv -v --volume=50 --force-window" }

$yt_page = Invoke-WebRequest `
  -UseBasicParsing  `
  -Uri $url `
  -UserAgent "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:135.0) Gecko/20100101 Firefox/135.0" `
  -Headers @{
    "Accept" = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
    "Accept-Language" = "en-US,en;q=0.5"
    "Accept-Encoding" = "gzip, deflate, br, zstd"
  }

$yt_initial_data = Select-String `
  -Pattern "(?<=var ytInitialData =)(.*?)(?=;</script?)" `
  -InputObject $yt_page.Content 
  | ForEach-Object { $_.Matches.Value }

$yt_initial_data
| jq -r ('[ .contents | .. | .videoRenderer? | select(. !=null) |
          { scraper: "youtube_search",
            url: "https://www.youtube.com/watch?v=\(.videoId)",
            title: .title.runs[0].text,
            channel: .longBylineText.runs[0].text,
            duration:.lengthText.simpleText,
            views: .shortViewCountText.simpleText,
            date: .publishedTimeText.simpleText,
            description: .detailedMetadataSnippets[0].snippetText.runs[0].text,
            id: .videoId,
            thumbnail: .thumbnail.thumbnails[0].url } ] ' -replace '\r\n', ' ')
| jq -c -r 'select(.!=[])|.[]' 
| jq -r '"\(.title)\t\(.channel)\t\(.duration)\t\(.views)\t\(.date)\t\(.id)\t\(.url)"' 
| fzf --sync --reverse --height=-1 `
      --with-nth=1 `
      --delimiter '\t' `
      --preview-window 'right,40%' `
      --preview 'curl -s https://img.youtube.com/vi/{6}/hqdefault.jpg | chafa -f sixels -s %FZF_PREVIEW_COLUMNS%x%FZF_PREVIEW_COLUMNS%' `
      --bind 'ctrl-w:toggle-preview' `
      --bind "ctrl-o:become($($mpv_cmd) {7})" `
      --bind "enter:become($($mpv_cmd) {7})"