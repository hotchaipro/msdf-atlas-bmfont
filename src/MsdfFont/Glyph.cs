using System;

namespace HotChai.Fonts.Msdf
{
    internal readonly struct Glyph
    {
        public Glyph(
            int unicode,
            double advance,
            Bounds planeBounds,
            Bounds atlasBounds)
        {
            this.Unicode = unicode;
            this.Advance = advance;
            this.PlaneBounds = planeBounds;
            this.AtlasBounds = atlasBounds;
        }

        public readonly int Unicode;
        public readonly double Advance;
        public readonly Bounds PlaneBounds;
        public readonly Bounds AtlasBounds;
    }
}
