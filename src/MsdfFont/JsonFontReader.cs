using System;
using System.Text.Json;

namespace HotChai.Fonts.Msdf
{
    internal sealed class JsonFontReader
    {
        private static readonly JsonReaderOptions ReaderOptions = new JsonReaderOptions()
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        };

        public MsdfFont Read(
            string path)
        {
            var json = File.ReadAllBytes(path);
            var reader = new Utf8JsonReader(json, ReaderOptions);

            reader.Read();
            return this.Read(ref reader);
        }

        private MsdfFont Read(
            ref Utf8JsonReader reader)
        {
            Atlas atlas = default;
            var metrics = new List<Metrics>();
            var glyphs = new List<Glyph>();

            if (StartObject(ref reader))
            {
                while (MoveToNextMember(ref reader, out var memberKey))
                {
                    switch (memberKey)
                    {
                        case "atlas":
                            atlas = this.ReadAtlas(ref reader);
                            break;

                        case "metrics":
                            metrics.Add(this.ReadMetrics(ref reader));
                            break;

                        case "glyphs":
                            this.ReadGlyphs(ref reader, glyphs);
                            break;

                        case "variants":
                            if (StartArray(ref reader))
                            {
                                while (MoveToNextArrayValue(ref reader))
                                {
                                    if (StartObject(ref reader))
                                    {
                                        while (MoveToNextMember(ref reader, out var variantMemberKey))
                                        {
                                            switch (variantMemberKey)
                                            {
                                                case "metrics":
                                                    metrics.Add(this.ReadMetrics(ref reader));
                                                    break;

                                                case "glyphs":
                                                    this.ReadGlyphs(ref reader, glyphs);
                                                    break;

                                                default:
                                                    reader.Skip();
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }

                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
            }

            return new MsdfFont(
                atlas: atlas,
                metrics: metrics[0],
                glyphs: new ReadOnlyMemory<Glyph>(glyphs.ToArray()));
        }

        private Atlas ReadAtlas(
            ref Utf8JsonReader reader)
        {
            string atlasType = null;
            double distanceRange = 0;
            double distanceRangeMiddle = 0;
            double size = 0;
            int width = 0;
            int height = 0;
            bool isTopYOrigin = false;

            if (StartObject(ref reader))
            {
                while (MoveToNextMember(ref reader, out var memberKey))
                {
                    switch (memberKey)
                    {
                        case "type":
                            atlasType = reader.GetString();
                            break;

                        case "distanceRange":
                            distanceRange = reader.GetDouble();
                            break;

                        case "distanceRangeMiddle":
                            distanceRangeMiddle = reader.GetDouble();
                            break;

                        case "size":
                            size = reader.GetDouble();
                            break;

                        case "width":
                            width = reader.GetInt32();
                            break;

                        case "height":
                            height = reader.GetInt32();
                            break;

                        case "yOrigin":
                            var yOrigin = reader.GetString();
                            if (string.Equals(yOrigin, "top", StringComparison.OrdinalIgnoreCase))
                            {
                                isTopYOrigin = true;
                            }
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
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
            ref Utf8JsonReader reader)
        {
            double emSize = 0;
            double lineHeight = 0;
            double ascender = 0;
            double descender = 0;
            double underlineY = 0;
            double underlineThickness = 0;

            if (StartObject(ref reader))
            {
                while (MoveToNextMember(ref reader, out var memberKey))
                {
                    switch (memberKey)
                    {
                        case "emSize":
                            emSize = reader.GetDouble();
                            break;

                        case "lineHeight":
                            lineHeight = reader.GetDouble();
                            break;

                        case "ascender":
                            ascender = reader.GetDouble();
                            break;

                        case "descender":
                            descender = reader.GetDouble();
                            break;

                        case "underlineY":
                            underlineY = reader.GetDouble();
                            break;

                        case "underlineThickness":
                            underlineThickness = reader.GetDouble();
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
            }

            return new Metrics(
                emSize: emSize,
                lineHeight: lineHeight,
                ascender: ascender,
                descender: descender,
                underlineY: underlineY,
                underlineThickness: underlineThickness);
        }

        private void ReadGlyphs(
            ref Utf8JsonReader reader,
            List<Glyph> glyphs)
        {
            if (StartArray(ref reader))
            {
                while (MoveToNextArrayValue(ref reader))
                {
                    glyphs.Add(this.ReadGlyph(ref reader));
                }
            }
        }

        private Glyph ReadGlyph(
            ref Utf8JsonReader reader)
        {
            int unicode = 0;
            double advance = 0;
            Bounds planeBounds = default;
            Bounds atlasBounds = default;

            if (StartObject(ref reader))
            {
                while (MoveToNextMember(ref reader, out var memberKey))
                {
                    switch (memberKey)
                    {
                        case "unicode":
                            unicode = reader.GetInt32();
                            break;

                        case "advance":
                            advance = reader.GetDouble();
                            break;

                        case "planeBounds":
                            planeBounds = this.ReadBounds(ref reader);
                            break;

                        case "atlasBounds":
                            atlasBounds = this.ReadBounds(ref reader);
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
            }

            return new Glyph(
                unicode: unicode,
                advance: advance,
                planeBounds: planeBounds,
                atlasBounds: atlasBounds);
        }

        private Bounds ReadBounds(
            ref Utf8JsonReader reader)
        {
            double left = 0;
            double top = 0;
            double right = 0;
            double bottom = 0;

            if (StartObject(ref reader))
            {
                while (MoveToNextMember(ref reader, out var memberKey))
                {
                    switch (memberKey)
                    {
                        case "left":
                            left = reader.GetDouble();
                            break;

                        case "top":
                            top = reader.GetDouble();
                            break;

                        case "right":
                            right = reader.GetDouble();
                            break;

                        case "bottom":
                            bottom = reader.GetDouble();
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
            }

            return new Bounds(
                left: left,
                top: top,
                right: right,
                bottom: bottom);
        }

        // The helpers below expect the reader to be positioned on the current value's first token
        // and leave it on the value's last token, as Utf8JsonReader.Skip does.

        /// <summary>
        /// Returns <c>true</c> if the current value is an object, or <c>false</c> if it is null.
        /// </summary>
        private static bool StartObject(
            ref Utf8JsonReader reader)
        {
            return reader.TokenType switch
            {
                JsonTokenType.StartObject => true,
                JsonTokenType.Null => false,
                _ => throw new JsonException($"Expected an object but found {reader.TokenType}."),
            };
        }

        /// <summary>
        /// Returns <c>true</c> if the current value is an array, or <c>false</c> if it is null.
        /// </summary>
        private static bool StartArray(
            ref Utf8JsonReader reader)
        {
            return reader.TokenType switch
            {
                JsonTokenType.StartArray => true,
                JsonTokenType.Null => false,
                _ => throw new JsonException($"Expected an array but found {reader.TokenType}."),
            };
        }

        /// <summary>
        /// Advances to the value of the next member of the current object, or returns <c>false</c>
        /// at the end of the object.
        /// </summary>
        private static bool MoveToNextMember(
            ref Utf8JsonReader reader,
            out string memberKey)
        {
            ReadToken(ref reader);

            if (reader.TokenType == JsonTokenType.EndObject)
            {
                memberKey = null;
                return false;
            }

            memberKey = reader.GetString();
            ReadToken(ref reader);
            return true;
        }

        /// <summary>
        /// Advances to the next value of the current array, or returns <c>false</c> at the end of
        /// the array.
        /// </summary>
        private static bool MoveToNextArrayValue(
            ref Utf8JsonReader reader)
        {
            ReadToken(ref reader);
            return reader.TokenType != JsonTokenType.EndArray;
        }

        private static void ReadToken(
            ref Utf8JsonReader reader)
        {
            if (!reader.Read())
            {
                throw new JsonException("Unexpected end of JSON.");
            }
        }
    }
}
