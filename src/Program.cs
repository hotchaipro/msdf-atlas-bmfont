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
            ConsoleApp.RegisterCommand("convert", ConvertCommand, "{input_path} -image:{atlas_image_path}");

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

            var msdfFontReader = new JsonFontReader();
            var msdfFont = msdfFontReader.Read(sourcePath);

            var bitmapFont = new BitmapFont();

            bitmapFont.Info = new BitmapFontInfo()
            {
                FontName = "",
                Size = (int)Math.Round(msdfFont.Atlas.Size),
                Unicode = true,
                Smooth = true,
            };

            bitmapFont.Common = new BitmapFontCommon()
            {
                // See https://www.angelcode.com/products/bmfont/doc/render_text.html
                LineHeight = (int)Math.Round(msdfFont.Metrics.LineHeight * msdfFont.Atlas.Size),
                Base = (int)Math.Round((0 - msdfFont.Metrics.Ascender) * msdfFont.Atlas.Size),
                ScaleWidth = msdfFont.Atlas.Width,
                ScaleHeight = msdfFont.Atlas.Height,
            };

            int page = 0;

            bitmapFont.Pages = new Dictionary<int, string>();
            bitmapFont.Pages[page] = image;

            bitmapFont.Characters = new Dictionary<int, Character>();

            foreach (var glyph in msdfFont.Glyphs.Span)
            {
                bitmapFont.Characters[glyph.Unicode] = new Character()
                {
                    // See https://www.angelcode.com/products/bmfont/doc/render_text.html
                    X = (int)Math.Round(glyph.AtlasBounds.Left),
                    Y = (int)Math.Round(glyph.AtlasBounds.Top),
                    Width = (int)Math.Round(glyph.AtlasBounds.Right - glyph.AtlasBounds.Left),
                    Height = (int)Math.Round(glyph.AtlasBounds.Bottom - glyph.AtlasBounds.Top),
                    XOffset = (int)Math.Round(glyph.PlaneBounds.Left * msdfFont.Atlas.Size),
                    // NOTE: PlaneBounds represents the glyph quad's bounds in em's relative to the baseline
                    // NOTE: YOffset represents the offset of the quad's bounds in pixels relative to the top of the line
                    YOffset = (int)Math.Round((glyph.PlaneBounds.Top - msdfFont.Metrics.Ascender) * msdfFont.Atlas.Size),
                    XAdvance = (int)Math.Round(glyph.Advance * msdfFont.Atlas.Size),
                    Page = page,
                    Channel = Channel.All,
                };
            }

            var writer = new XmlFontWriter();
            writer.WriteFont(bitmapFont, destinationPath);

            Console.WriteLine($"Wrote {destinationPath}");

            return ConsoleResult.Success;
        }
    }
}
