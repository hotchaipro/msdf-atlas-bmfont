using System;

namespace HotChai.Fonts.Bitmap
{
    internal sealed class BitmapFont
    {
        public BitmapFontInfo Info
        {
            get; set;
        }

        public BitmapFontCommon Common
        {
            get; set;
        }

        public IDictionary<int, string> Pages
        {
            get; set;
        }

        public IDictionary<int, Character> Characters
        {
            get; set;
        }

        public IDictionary<KerningPair, int> KerningPairs
        {
            get; set;
        }

        /// <summary>
        /// The distance field parameters, or <c>null</c> if the atlas is not a distance field.
        /// </summary>
        public DistanceField DistanceField
        {
            get; set;
        }
    }
}
