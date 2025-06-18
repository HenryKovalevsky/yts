param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

dotnet publish src\yts.fsproj --sc -o publish
Copy-Item src\yts.ps1 publish\yts.ps1
Compress-Archive -Force publish scoop\publish.zip

$hash = (Get-FileHash -Path scoop\publish.zip -Algorithm SHA256).Hash.ToLower()
$json = Get-Content scoop\yts.json -Raw | ConvertFrom-Json
$json.hash = $hash
$json.version = $Version
$json | ConvertTo-Json -Depth 4 | Set-Content scoop\yts.json -Encoding UTF8