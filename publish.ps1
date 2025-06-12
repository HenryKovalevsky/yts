dotnet publish src\yts.fsproj --sc -o publish
Copy-Item src\yts.ps1 publish\yts.ps1
Compress-Archive -Force publish scoop\publish.zip