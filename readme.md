# Image size reader
Getting image dimensions without reading an entire file.

- No GDI+ involved here, only pure c#
- Optimized for memory consuption and being cloud friendly
- Early stop when it cannot determine the dimensions

# Install from Nuget
To get the latest version:
```
Install-Package ImageSizeReader
```

# How to use

```
import ImageSizeReader;
import ImageSizeReader.Model;

// ...

Stream fileStream = File.OpenRead(file);
IImageSizeReaderUtil util = new ImageSizeReaderUtil();
var dimensions = util.GetDimensions(fileStream);
// dimensions.Width
// dimensions.Height
```

## Notes
Originally, this is based on https://stackoverflow.com/a/112711 and associated comments to handle edge cases.

It has been tested with lot of different images. For example the [ImageSharp repo](https://github.com/SixLabors/ImageSharp) has many useful images for testing.

In order to improve the Jfif (aka JPEG File Interchange Format) decoding, the following page has been useful:
http://vip.sugovica.hu/Sardi/kepnezo/JPEG%20File%20Layout%20and%20Format.htm

# Copyright and license
Code released under the MIT license.
