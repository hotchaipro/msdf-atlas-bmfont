using System;

namespace HotChai.Fonts.Msdf
{
    internal readonly struct Metrics
    {
        public Metrics(
            double emSize,
            double lineHeight,
            double ascender,
            double descender,
            double underlineY,
            double underlineThickness)
        {
            this.EmSize = emSize;
            this.LineHeight = lineHeight;
            this.Ascender = ascender;
            this.Descender = descender;
            this.UnderlineY = underlineY;
            this.UnderlineThickness = underlineThickness;
        }

        public readonly double EmSize;
        public readonly double LineHeight;
        public readonly double Ascender;
        public readonly double Descender;
        public readonly double UnderlineY;
        public readonly double UnderlineThickness;
    }
}
