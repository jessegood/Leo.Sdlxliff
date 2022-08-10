# Leo.Sdlxliff
A parser for SDLXLIFF files (uses .NET 6)

# Remove all comments
```
var sdlxliff = await SdlxliffDocument.LoadAsync(fileName);
sdlxliff.RemoveAllComments();
await sdlxliffWriter.SaveAsAsync(fileName);
```

# Find target that contains phrase
```
var sdlxliff = await SdlxliffDocument.LoadAsync(stream);
var pairs = sdlxliff.FindSegmentPairs(null, "shock absorber");
```

# Find segments that contain revisions
```
var sdlxliff = await SdlxliffDocument.LoadAsync(stream);
var revisionPairs = sdlxliff.FindSegmentPairs(null, null, new SegmentSearchSettings
{
    WithRevisions = true
});
```

