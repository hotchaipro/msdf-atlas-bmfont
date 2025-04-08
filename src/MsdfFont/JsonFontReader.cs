using System;
using HotChai.Fonts.Msdf;
using HotChai.Json;

namespace HotChai.Fonts.Msdf
{
    internal sealed class JsonFontReader
    {
        public MsdfFont Read(
            string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var reader = new JsonReader(stream);
                return this.Read(reader);
            }
        }

        private MsdfFont Read(
            JsonReader reader)
        {
            Atlas atlas = default;
            Metrics metrics = default;
            ReadOnlyMemory<Glyph> glyphs = null;

            if (reader.ReadStartObject())
            {
                while (reader.MoveToNextMember())
                {
                    switch (reader.MemberKey)
                    {
                        case "atlas":
                            atlas = this.ReadAtlas(reader);
                            break;

                        case "metrics":
                            metrics = this.ReadMetrics(reader);
                            break;

                        case "glyphs":
                            var glyphsList = new List<Glyph>(this.ReadGlyphs(reader));
                            glyphs = new ReadOnlyMemory<Glyph>(glyphsList.ToArray());
                            break;
                    }
                }

                reader.ReadEndObject();
            }

            return new MsdfFont(
                atlas: atlas,
                metrics: metrics,
                glyphs: glyphs);
        }

        private Atlas ReadAtlas(
            JsonReader reader)
        {
            string atlasType = null;
            int distanceRange = 0;
            int distanceRangeMiddle = 0;
            double size = 0;
            int width = 0;
            int height = 0;
            bool isTopYOrigin = false;

            if (reader.ReadStartObject())
            {
                while (reader.MoveToNextMember())
                {
                    switch (reader.MemberKey)
                    {
                        case "type":
                            atlasType = reader.ReadValueAsString(64);
                            break;

                        case "distanceRange":
                            distanceRange = reader.ReadValueAsInt32();
                            break;

                        case "distanceRangeMiddle":
                            distanceRangeMiddle = reader.ReadValueAsInt32();
                            break;

                        case "size":
                            size = reader.ReadValueAsDouble();
                            break;

                        case "width":
                            width = reader.ReadValueAsInt32();
                            break;

                        case "height":
                            height = reader.ReadValueAsInt32();
                            break;

                        case "yOrigin":
                            var yOrigin = reader.ReadValueAsString(64);
                            if (0 == String.Compare(yOrigin, "top", StringComparison.OrdinalIgnoreCase))
                            {
                                isTopYOrigin = true;
                            }
                            break;
                    }
                }

                reader.ReadEndObject();
            }

            return new Atlas(
                atlasType: atlasType,
                distanceRange: distanceRange,
                distanceRangeMiddle: distanceRangeMiddle,
                size: size,
                width: width,
                height: height,
                isTopYOrigin: isTopYOrigin);
        }

        private Metrics ReadMetrics(
            JsonReader reader)
        {
            double emSize = 0;
            double lineHeight = 0;
            double ascender = 0;
            double descender = 0;
            double underlineY = 0;
            double underlineThickness = 0;

            if (reader.ReadStartObject())
            {
                while (reader.MoveToNextMember())
                {
                    switch (reader.MemberKey)
                    {
                        case "emSize":
                            emSize = reader.ReadValueAsDouble();
                            break;

                        case "lineHeight":
                            lineHeight = reader.ReadValueAsDouble();
                            break;

                        case "ascender":
                            ascender = reader.ReadValueAsDouble();
                            break;

                        case "descender":
                            descender = reader.ReadValueAsDouble();
                            break;

                        case "underlineY":
                            underlineY = reader.ReadValueAsDouble();
                            break;

                        case "underlineThickness":
                            underlineThickness = reader.ReadValueAsDouble();
                            break;
                    }
                }

                reader.ReadEndObject();
            }

            return new Metrics(
                emSize: emSize,
                lineHeight: lineHeight,
                ascender: ascender,
                descender: descender,
                underlineY: underlineY,
                underlineThickness: underlineThickness);
        }

        private IEnumerable<Glyph> ReadGlyphs(
            JsonReader reader)
        {
            if (reader.ReadStartArray())
            {
                while (reader.MoveToNextArrayValue())
                {
                    yield return this.ReadGlyph(reader);
                }

                reader.ReadEndArray();
            }
        }

        private Glyph ReadGlyph(
            JsonReader reader)
        {
            int unicode = 0;
            double advance = 0;
            Bounds planeBounds = default;
            Bounds atlasBounds = default;

            if (reader.ReadStartObject())
            {
                while (reader.MoveToNextMember())
                {
                    switch (reader.MemberKey)
                    {
                        case "unicode":
                            unicode = reader.ReadValueAsInt32();
                            break;

                        case "advance":
                            advance = reader.ReadValueAsDouble();
                            break;

                        case "planeBounds":
                            planeBounds = this.ReadBounds(reader);
                            break;

                        case "atlasBounds":
                            atlasBounds = this.ReadBounds(reader);
                            break;
                    }
                }

                reader.ReadEndObject();
            }

            return new Glyph(
                unicode: unicode,
                advance: advance,
                planeBounds: planeBounds,
                atlasBounds: atlasBounds);
        }

        private Bounds ReadBounds(
            JsonReader reader)
        {
            double left = 0;
            double top = 0;
            double right = 0;
            double bottom = 0;

            if (reader.ReadStartObject())
            {
                while (reader.MoveToNextMember())
                {
                    switch (reader.MemberKey)
                    {
                        case "left":
                            left = reader.ReadValueAsDouble();
                            break;

                        case "top":
                            top = reader.ReadValueAsDouble();
                            break;

                        case "right":
                            right = reader.ReadValueAsDouble();
                            break;

                        case "bottom":
                            bottom = reader.ReadValueAsDouble();
                            break;
                    }
                }

                reader.ReadEndObject();
            }

            return new Bounds(
                left: left,
                top: top,
                right: right,
                bottom: bottom);
        }
    }
}
