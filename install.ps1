$outDir="$PSScriptRoot\.pack"
$projectFile = (Get-ChildItem -Path "$PSScriptRoot\src" -Filter "*.csproj").FullName

if (Test-Path -Path $outDir){
    Remove-Item -Path $outDir/* -Recurse
}

dotnet pack $projectFile --output $outDir --include-symbols

$nuget="$env:USERPROFILE\.nuget\packages"
$packageFileName=(Get-ChildItem -Path $outDir -Exclude *symbols.nupkg).Name
$match=[System.Text.RegularExpressions.Regex]::Match($packageFileName, "^([^.]+)\.(.*?)(?=.nupkg)")
$packageId=$match.Groups[1].Value
$version=$match.Groups[2].Value
$targetPath = "$nuget\$packageId\$version"

if (Test-Path -Path $targetPath){
    Remove-Item $targetPath -Recurse
}

dotnet nuget push "$outDir\$packageFileName" -s $nuget
Remove-Item -Path $outDir -Recurse