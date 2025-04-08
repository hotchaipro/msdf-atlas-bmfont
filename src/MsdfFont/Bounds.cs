using System;

namespace HotChai.Fonts.Msdf
{
    internal readonly struct Bounds
    {
        public Bounds(
            double left,
            double top,
            double right,
            double bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public readonly double Left;
        public readonly double Top;
        public readonly double Right;
        public readonly double Bottom;
    }
}
