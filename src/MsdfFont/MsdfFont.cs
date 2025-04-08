using System;

namespace HotChai.Fonts.Msdf
{
    internal sealed class MsdfFont
    {
        public MsdfFont(
            Atlas atlas,
            Metrics metrics,
            ReadOnlyMemory<Glyph> glyphs)
        {
            this.Atlas = atlas;
            this.Metrics = metrics;
            this.Glyphs = glyphs;
        }

        public readonly Atlas Atlas;
        public readonly Metrics Metrics;
        public readonly ReadOnlyMemory<Glyph> Glyphs;
    }
}
