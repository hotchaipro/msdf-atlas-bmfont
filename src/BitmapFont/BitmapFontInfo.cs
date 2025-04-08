using System;

namespace HotChai.Fonts.Bitmap
{
    internal sealed class BitmapFontInfo
    {
        public int Size
        {
            get; set;
        }

        public bool Smooth
        {
            get; set;
        }

        public bool Unicode
        {
            get; set;
        }

        public bool Italic
        {
            get; set;
        }

        public bool Bold
        {
            get; set;
        }

        public bool FixedHeight
        {
            get; set;
        }

        public string Charset
        {
            get; set;
        }

        public int StretchHeight
        {
            get; set;
        }

        public int SuperSamplingLevel
        {
            get; set;
        }

        public int PaddingUp
        {
            get; set;
        }

        public int PaddingDown
        {
            get; set;
        }

        public int PaddingLeft
        {
            get; set;
        }

        public int PaddingRight
        {
            get; set;
        }

        public int SpacingHorizontal
        {
            get; set;
        }

        public int SpacingVertical
        {
            get; set;
        }

        public int Outline
        {
            get; set;
        }

        public string FontName
        {
            get; set;
        }
    }
}
