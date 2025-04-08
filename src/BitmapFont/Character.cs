using System;

namespace HotChai.Fonts.Bitmap
{
    internal sealed class Character
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int XOffset { get; set; }

        public int YOffset { get; set; }

        public int XAdvance { get; set; }

        public int Page { get; set; }

        public Channel Channel { get; set; }
    }
}
