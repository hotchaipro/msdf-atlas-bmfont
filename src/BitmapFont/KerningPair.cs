using System;

namespace HotChai.Fonts.Bitmap
{
    internal readonly struct KerningPair
    {
        public KerningPair(
            int first,
            int second)
        {
            this.First = first;
            this.Second = second;
        }

        public int First
        {
            get;
        }

        public int Second
        {
            get;
        }
    }
}
