using System;

namespace HotChai.Fonts.Bitmap
{
    /// <summary>
    /// The distance field parameters of a font whose atlas stores signed distances instead of
    /// coverage, which a renderer needs in order to draw the glyphs.
    /// </summary>
    /// <remarks>
    /// Written in the binary format as <see cref="BlockId.DistanceField"/>, little-endian:
    /// <code>
    /// 4 bytes  signature  "MSDF"
    /// 1 byte   version    1
    /// 1 byte   type       see <see cref="DistanceFieldType"/>
    /// 4 bytes  float      distance range, in atlas pixels
    /// 4 bytes  float      distance range middle, in atlas pixels
    /// </code>
    /// Later versions may append fields; a reader that understands an earlier version reads the
    /// fields it knows and skips the rest of the block.
    /// </remarks>
    internal sealed class DistanceField
    {
        public static ReadOnlySpan<byte> Signature => "MSDF"u8;

        /// <summary>
        /// The version this code writes.
        /// </summary>
        public const byte Version = 1;

        /// <summary>
        /// The size of the block this code writes.
        /// </summary>
        public const int SizeInBytes = 14;

        /// <summary>
        /// The earliest version a reader accepts; later versions only append fields.
        /// </summary>
        public const byte MinimumVersion = 1;

        /// <summary>
        /// The smallest block a reader accepts: the fields of <see cref="MinimumVersion"/>.
        /// </summary>
        public const int MinimumSizeInBytes = 14;

        public DistanceFieldType Type
        {
            get; set;
        }

        /// <summary>
        /// The width of the range of distances encoded in the atlas, in atlas pixels (msdf-atlas-gen's
        /// <c>-pxrange</c>).
        /// </summary>
        public float DistanceRange
        {
            get; set;
        }

        /// <summary>
        /// The signed distance, in atlas pixels, encoded at the middle of the range (msdf-atlas-gen's
        /// <c>distanceRangeMiddle</c>), with positive distances inside the glyphs. It is zero for a
        /// symmetric range (<c>-pxrange</c>) and nonzero for an asymmetric one (<c>-apxrange</c>).
        /// A texel value <c>v</c> in [0, 1] encodes the distance
        /// <c>(v - 0.5) * DistanceRange + DistanceRangeMiddle</c>.
        /// </summary>
        public float DistanceRangeMiddle
        {
            get; set;
        }

        public static DistanceFieldType ParseType(
            string atlasType)
        {
            return atlasType?.ToLowerInvariant() switch
            {
                "sdf" => DistanceFieldType.Sdf,
                "psdf" => DistanceFieldType.Psdf,
                "msdf" => DistanceFieldType.Msdf,
                "mtsdf" => DistanceFieldType.Mtsdf,
                _ => DistanceFieldType.Unknown,
            };
        }
    }

    internal enum DistanceFieldType : byte
    {
        Unknown = 0,
        Sdf = 1,
        Psdf = 2,
        Msdf = 3,
        Mtsdf = 4,
    }
}
