#nullable enable

using System;
using HotChai.CommandLine;
using HotChai.Fonts.Bitmap;
using HotChai.Fonts.Msdf;

namespace HotChai.Fonts
{
    public sealed class Program
    {
        private static Task<int> Main(
            string[] args)
        {
            ConsoleApp.RegisterCommand("convert", ConvertCommand, "{input_path} -image:{atlas_image_path} [-format:xml|binary]");

            return ConsoleApp.Start(args);
        }

        private static int ConvertCommand(
            string[] parameters,
            Dictionary<string, string?> flags)
        {
            if (parameters.Length != 1)
            {
                return ConsoleResult.SyntaxError;
            }

            var sourcePath = parameters[0];
            var destinationPath = Path.ChangeExtension(sourcePath, ".fnt");

            var image = flags["image"];

            bool binaryFormat = false;
            if (flags.TryGetValue("format", out var format))
            {
                binaryFormat = format?.Equals("binary", StringComparison.OrdinalIgnoreCase) ?? false;
            }

            var msdfFontReader = new JsonFontReader();
            var msdfFont = msdfFontReader.Read(sourcePath);

            var bitmapFont = new BitmapFont();

            var atlas = msdfFont.Atlas;
            var pixelsPerUnit = atlas.Size;

            // BMFont measures y downward from the top; normalize a bottom-origin atlas to match.
            double ySign = atlas.IsTopYOrigin ? 1 : -1;
            var ascender = ySign * msdfFont.Metrics.Ascender;

            // -fontscale scales the geometry, so an em spans EmSize units.
            var emSize = (msdfFont.Metrics.EmSize > 0) ? msdfFont.Metrics.EmSize : 1;

            bitmapFont.Info = new BitmapFontInfo()
            {
                FontName = "",
                Size = RoundToPixel(pixelsPerUnit * emSize),
                Unicode = true,
                Smooth = true,
            };

            // Glyph y offsets are measured from this rounded baseline so that every glyph sits on it.
            var baseline = RoundToPixel(-ascender * pixelsPerUnit);

            bitmapFont.Common = new BitmapFontCommon()
            {
                // See https://www.angelcode.com/products/bmfont/doc/render_text.html
                LineHeight = RoundToPixel(msdfFont.Metrics.LineHeight * pixelsPerUnit),
                Base = baseline,
                ScaleWidth = atlas.Width,
                ScaleHeight = atlas.Height,
            };

            var distanceFieldType = DistanceField.ParseType(msdfFont.Atlas.AtlasType);
            if ((distanceFieldType != DistanceFieldType.Unknown) && (msdfFont.Atlas.DistanceRange > 0))
            {
                bitmapFont.DistanceField = new DistanceField()
                {
                    Type = distanceFieldType,
                    DistanceRange = (float)msdfFont.Atlas.DistanceRange,
                };
            }

            int page = 0;

            bitmapFont.Pages = new Dictionary<int, string>();
            bitmapFont.Pages[page] = image;

            bitmapFont.Characters = new Dictionary<int, Character>();

            foreach (var glyph in msdfFont.Glyphs.Span)
            {
                var character = new Character()
                {
                    XAdvance = RoundToPixel(glyph.Advance * pixelsPerUnit),
                    Page = page,
                    Channel = Channel.All,
                };

                var atlasBounds = glyph.AtlasBounds;
                var planeBounds = glyph.PlaneBounds;

                // Whitespace glyphs have no bounds and draw nothing.
                if ((atlasBounds.Right > atlasBounds.Left) && (atlasBounds.Top != atlasBounds.Bottom))
                {
                    double atlasTop = atlas.IsTopYOrigin ? atlasBounds.Top : atlas.Height - atlasBounds.Top;
                    double atlasBottom = atlas.IsTopYOrigin ? atlasBounds.Bottom : atlas.Height - atlasBounds.Bottom;
                    double planeTop = ySign * planeBounds.Top;

                    // msdf-atlas-gen insets the atlas bounds by half a texel (to texel centers).
                    // Expand them to whole texels, and move the quad by the same amount, so the
                    // texels map onto the quad exactly as msdf-atlas-gen laid them out.
                    int x = (int)Math.Floor(atlasBounds.Left);
                    int y = (int)Math.Floor(atlasTop);

                    character.X = x;
                    character.Y = y;
                    character.Width = (int)Math.Ceiling(atlasBounds.Right) - x;
                    character.Height = (int)Math.Ceiling(atlasBottom) - y;
                    character.XOffset = RoundToPixel((planeBounds.Left * pixelsPerUnit) - (atlasBounds.Left - x));
                    // YOffset is measured from the top of the line; planeTop is relative to the baseline.
                    character.YOffset = baseline + RoundToPixel((planeTop * pixelsPerUnit) - (atlasTop - y));
                }

                bitmapFont.Characters[glyph.Unicode] = character;
            }

            if (binaryFormat)
            {
                var writer = new BinaryFontWriter();
                writer.WriteFont(bitmapFont, destinationPath);
            }
            else
            {
                var writer = new XmlFontWriter();
                writer.WriteFont(bitmapFont, destinationPath);
            }

            Console.WriteLine($"Wrote {destinationPath}");

            return ConsoleResult.Success;
        }

        private static int RoundToPixel(
            double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}
