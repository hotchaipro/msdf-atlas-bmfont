//**************************************************************************************************
// BitmapFont.cs                                                                                   *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using System;

namespace HotChai.Fonts.Bitmap
{
    internal sealed class TextFontWriter
    {
        public void WriteFont(
            BitmapFont font,
            TextWriter textWriter)
        {
            // Info

            if (font.Info != null)
            {
                textWriter.Write("info");
                this.WriteInfo(font.Info, textWriter);
                textWriter.WriteLine();
            }

            // Common

            if (font.Common != null)
            {
                textWriter.Write("common");
                this.WriteCommon(font.Common, textWriter, font.Pages.Count);
                textWriter.WriteLine();
            }

            // Pages

            if (font.Pages != null)
            {
                foreach (var page in font.Pages)
                {
                    textWriter.Write("page");
                    WriteInt("id", page.Key, textWriter);
                    WriteString("file", page.Value, textWriter);
                    textWriter.WriteLine();
                }
            }

            // Characters

            if (font.Characters != null)
            {
                textWriter.Write("chars");
                WriteInt("count", font.Characters.Count, textWriter);
                textWriter.WriteLine();

                foreach (var keyValuePair in font.Characters)
                {
                    textWriter.Write("char");
                    WriteCharacter(keyValuePair.Value, textWriter, keyValuePair.Key);
                    textWriter.WriteLine();
                }

            }

            // Kernings

            if (font.KerningPairs != null && font.KerningPairs.Count > 0)
            {
                textWriter.Write("kernings");
                WriteInt("count", font.KerningPairs.Count, textWriter);
                textWriter.WriteLine();

                foreach (var keyValuePair in font.KerningPairs)
                {
                    textWriter.Write("kerning");
                    WriteKerningPair(keyValuePair.Key, textWriter, keyValuePair.Value);
                    textWriter.WriteLine();
                }
            }
        }

        private void WriteCommon(
            BitmapFontCommon common,
            TextWriter textWriter,
            int pages)
        {
            WriteInt("lineHeight", common.LineHeight, textWriter);
            WriteInt("base", common.Base, textWriter);
            WriteInt("scaleW", common.ScaleWidth, textWriter);
            WriteInt("scaleH", common.ScaleHeight, textWriter);

            WriteInt("pages", pages, textWriter);

            WriteBool("packed", common.Packed, textWriter);

            WriteEnum("alphaChnl", common.AlphaChannel, textWriter);
            WriteEnum("redChnl", common.RedChannel, textWriter);
            WriteEnum("greenChnl", common.GreenChannel, textWriter);
            WriteEnum("blueChnl", common.BlueChannel, textWriter);
        }

        private void WriteInfo(
            BitmapFontInfo info,
            TextWriter textWriter)
        {
            WriteString("face", info.FontName ?? string.Empty, textWriter);
            WriteInt("size", info.Size, textWriter);
            WriteBool("bold", info.Bold, textWriter);
            WriteBool("italic", info.Italic, textWriter);

            WriteString("charset", info.Charset ?? string.Empty, textWriter);

            WriteBool("unicode", info.Unicode, textWriter);
            WriteInt("stretchH", info.StretchHeight, textWriter);
            WriteBool("smooth", info.Smooth, textWriter);
            WriteInt("aa", info.SuperSamplingLevel, textWriter);

            var padding = $"{info.PaddingUp},{info.PaddingRight},{info.PaddingDown},{info.PaddingLeft}";
            WriteValue("padding", padding, textWriter);

            var spacing = $"{info.SpacingHorizontal},{info.SpacingVertical}";
            WriteValue("spacing", spacing, textWriter);

            WriteInt("outline", info.Outline, textWriter);
        }

        private void WriteCharacter(
            Character character,
            TextWriter textWriter,
            int id)
        {
            WriteInt("id", id, textWriter);
            WriteInt("x", character.X, textWriter);
            WriteInt("y", character.Y, textWriter);
            WriteInt("width", character.Width, textWriter);
            WriteInt("height", character.Height, textWriter);
            WriteInt("xoffset", character.XOffset, textWriter);
            WriteInt("yoffset", character.YOffset, textWriter);
            WriteInt("xadvance", character.XAdvance, textWriter);
            WriteInt("page", character.Page, textWriter);
            WriteEnum("chnl", character.Channel, textWriter);
        }

        private void WriteKerningPair(
            KerningPair kerningPair,
            TextWriter textWriter,
            int amount)
        {
            WriteInt("first", kerningPair.First, textWriter);
            WriteInt("second", kerningPair.Second, textWriter);
            WriteInt("amount", amount, textWriter);
        }

        private static void WriteValue(
            string propertyName,
            string value,
            TextWriter textWriter)
        {
            textWriter.Write(" {0}={1}", propertyName, value);
        }

        private static void WriteString(
            string propertyName,
            string value,
            TextWriter textWriter)
        {
            textWriter.Write(" {0}=\"{1}\"", propertyName, value);
        }

        private static void WriteInt(
            string propertyName,
            int value,
            TextWriter textWriter)
        {
            WriteValue(propertyName, value.ToString(), textWriter);
        }

        private static void WriteBool(
            string propertyName,
            bool value,
            TextWriter textWriter)
        {
            WriteValue(propertyName, value ? "1" : "0", textWriter);
        }

        private static void WriteEnum<T>(
            string propertyName,
            T value,
            TextWriter textWriter)
        {
            WriteInt(propertyName, Convert.ToInt32(value), textWriter);
        }
    }
}
