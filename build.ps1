# Clean
$nuget = "$env:USERPROFILE\.nuget\packages"
Remove-Item -Path "$nuget\vertical-spectreviewer" -Force -Recurse -ErrorAction SilentlyContinue
Remove-Item -Path .\src\bin\Release\*.nupkg

# Pack library
dotnet pack -c Release -p:VersionSuffix=dev .\src\Vertical.SpectreViewer.csproj

# Push local package
$nupkg = (Get-ChildItem -Path .\src\bin\Release\ -filter "*-dev.nupkg").FullName
dotnet nuget push "$nupkg" -s "$nuget"
 