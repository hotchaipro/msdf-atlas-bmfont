using System;

namespace HotChai.Fonts.Bitmap
{
    internal enum BlockId
    {
        Info = 1,
        Common = 2,
        Pages = 3,
        Characters = 4,
        KerningPairs = 5,

        /// <summary>
        /// An extension block that describes the distance field of an SDF, MSDF, or MTSDF
        /// font. See <see cref="Bitmap.DistanceField"/> for the layout.
        /// </summary>
        DistanceField = 200,
    }
}
