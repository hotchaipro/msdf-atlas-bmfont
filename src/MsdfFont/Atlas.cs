using System;

namespace HotChai.Fonts.Msdf
{
    internal readonly struct Atlas
    {
        public Atlas(
            string atlasType,
            int distanceRange,
            int distanceRangeMiddle,
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
        public readonly int DistanceRange;
        public readonly int DistanceRangeMiddle;
        public readonly double Size;
        public readonly int Width;
        public readonly int Height;
        public readonly bool IsTopYOrigin;
    }
}
