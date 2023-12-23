# Clean
$nuget = "$env:USERPROFILE\.nuget\packages"
Remove-Item -Path "$nuget\vertical-spectreviewer" -Force -Recurse
Remote-Item -Path .\src\bin\Release\*.nupkg

# Pack library
dotnet pack -c Release .\src\Vertical.SpectreViewer.csproj

# Push local package
$nupkg = (Get-ChildItem -Path .\src\bin\Release\ -filter "*-alpha.nupkg").FullName
dotnet nuget push "$nupkg" -s "$nuget"
 