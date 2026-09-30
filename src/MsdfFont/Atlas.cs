using System;

namespace HotChai.Fonts.Msdf
{
    internal readonly struct Atlas
    {
        public Atlas(
            string atlasType,
            double distanceRange,
            double distanceRangeMiddle,
            double size,
            int width,
            int height,
            bool isTopYOrigin)
        {
            this.AtlasType = atlasType;
            this.DistanceRange = distanceRange;
            this.DistanceRangeMiddle = distanceRangeMiddle;
            this.Size = size;
            this.Width = width;
            this.Height = height;
            this.IsTopYOrigin = isTopYOrigin;
        }

        public readonly string AtlasType;
        public readonly double DistanceRange;
        public readonly double DistanceRangeMiddle;
        /// <summary>
        /// The font size in pixels per em.
        /// </summary>
        public readonly double Size;
        public readonly int Width;
        public readonly int Height;
        public readonly bool IsTopYOrigin;
    }
}
