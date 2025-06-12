param(
  [Parameter(Mandatory, HelpMessage="Search query")]
  [string]$search_query
)

Push-Location $PSScriptRoot
$link = .\yts.exe $search_query
Pop-Location

if ($link) {
  mpv --volume=50 --force-window=immediate --geometry=50%x50% $link
}
