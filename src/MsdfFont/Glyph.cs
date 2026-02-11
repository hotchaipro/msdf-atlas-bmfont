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
        /// <summary>
        /// The horizontal distance to advance the cursor in em's.
        /// </summary>
        public readonly double Advance;
        /// <summary>
        /// The glyph quad's bounds in em's relative to the baseline and horizontal cursor position.
        /// </summary>
        public readonly Bounds PlaneBounds;
        /// <summary>
        /// The glyph's bounds in the atlas in pixels.
        /// </summary>
        public readonly Bounds AtlasBounds;
    }
}
