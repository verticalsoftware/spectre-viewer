# vertical-spectreviewer

A pageable content viewer that uses spectre console for rendering.

## Quick setup

Install:

```
> dotnet package add vertical-spectreview --preelease
```

Call one of the methods on the `SpectreViewer` class with content you would normally send to SpectreConsole. The example below provides content from a file.

```csharp
// Renders markup found in a file
SpectreViewer.MarkupWithPaging(File.OpenRead("marked-up.txt"));
```

The utility will display the content of the string or stream and let the user page through using `PageUp` and `PageDown` keys.

## A note on input

Recall spectre console markup - `[gold]...[/]`, etc. If you want to use brackets in the rendered content, be sure to properly escape them (e.g. `[[ my-escaped-content ]]`).