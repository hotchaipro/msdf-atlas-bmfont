using System;

namespace HotChai.Fonts.Bitmap
{
    internal sealed class BitmapFontCommon
    {
        public int LineHeight
        {
            get; set;
        }

        public int Base
        {
            get; set;
        }

        public int ScaleWidth
        {
            get; set;
        }

        public int ScaleHeight
        {
            get; set;
        }

        public bool Packed
        {
            get; set;
        }

        public ChannelData AlphaChannel
        {
            get; set;
        }

        public ChannelData RedChannel
        {
            get; set;
        }

        public ChannelData GreenChannel
        {
            get; set;
        }

        public ChannelData BlueChannel
        {
            get; set;
        }
    }
}
