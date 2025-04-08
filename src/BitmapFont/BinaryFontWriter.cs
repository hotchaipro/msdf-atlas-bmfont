//**************************************************************************************************
// BitmapFont.cs                                                                                   *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using System;

namespace HotChai.Fonts.Bitmap
{
    internal sealed class BinaryFontWriter
    {
        private const int ImplementedVersion = 3;

        private const byte MagicOne = 66;
        private const byte MagicTwo = 77;
        private const byte MagicThree = 70;

        private const int InfoMinSizeInBytes = 15;
        private const int CommonSizeInBytes = 15;
        private const int CharacterSizeInBytes = 20;
        private const int KerningPairSizeInBytes = 10;

        public void WriteFont(
            BitmapFont font,
            string path)
        {
            using (var stream = File.OpenWrite(path))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    this.WriteFont(font, writer);
                }
            }
        }

        private void WriteFont(
            BitmapFont font,
            BinaryWriter binaryWriter)
        {
            binaryWriter.Write(MagicOne);
            binaryWriter.Write(MagicTwo);
            binaryWriter.Write(MagicThree);
            binaryWriter.Write((byte)ImplementedVersion);

            if (font.Info != null)
            {
                binaryWriter.Write((byte)BlockId.Info);
                this.WriteInfo(font.Info, binaryWriter);
            }

            if (font.Common != null)
            {
                binaryWriter.Write((byte)BlockId.Common);
                this.WriteCommon(font.Common, binaryWriter, font.Pages.Count);
            }

            if (font.Pages != null)
            {
                binaryWriter.Write((byte)BlockId.Pages);
                binaryWriter.Write(font.Pages.Values.Sum(page => page.Length + 1));

                // Unlike the XML and text formats, the binary format requires page IDs to be consecutive and zero based. 

                var index = 0;
                foreach (var keyValuePair in font.Pages.OrderBy(pair => pair.Key))
                {
                    if (keyValuePair.Key != index)
                        throw new InvalidDataException("The binary format requires that page IDs be consecutive and zero based.");
                    WriteNullTerminatedString(binaryWriter, keyValuePair.Value);
                    index++;
                }
            }

            if (font.Characters != null)
            {
                binaryWriter.Write((byte)BlockId.Characters);
                binaryWriter.Write(font.Characters.Values.Count * CharacterSizeInBytes);

                foreach (var keyValuePair in font.Characters)
                {
                    WriteCharacter(keyValuePair.Value, binaryWriter, keyValuePair.Key);
                }
            }

            if (font.KerningPairs != null && font.KerningPairs.Count > 0)
            {
                binaryWriter.Write((byte)BlockId.KerningPairs);
                binaryWriter.Write(font.KerningPairs.Keys.Count * KerningPairSizeInBytes);

                foreach (var keyValuePair in font.KerningPairs)
                {
                    WriteKerningPair(keyValuePair.Key, binaryWriter, keyValuePair.Value);
                }
            }
        }

        private void WriteCommon(
            BitmapFontCommon common,
            BinaryWriter binaryWriter,
            int pages)
        {
            binaryWriter.Write(CommonSizeInBytes);
            binaryWriter.Write((ushort)common.LineHeight);
            binaryWriter.Write((ushort)common.Base);
            binaryWriter.Write((ushort)common.ScaleWidth);
            binaryWriter.Write((ushort)common.ScaleHeight);
            binaryWriter.Write((ushort)pages);

            byte packed = 0;
            packed = SetBit(packed, 0, common.Packed);
            binaryWriter.Write(packed);

            binaryWriter.Write((byte)common.AlphaChannel);
            binaryWriter.Write((byte)common.RedChannel);
            binaryWriter.Write((byte)common.GreenChannel);
            binaryWriter.Write((byte)common.BlueChannel);
        }

        private void WriteInfo(
            BitmapFontInfo info,
            BinaryWriter binaryWriter)
        {
            binaryWriter.Write(InfoMinSizeInBytes + (info.FontName?.Length ?? 0));
            binaryWriter.Write((short)info.Size);

            byte bitField = 0;

            bitField = SetBit(bitField, 7, info.Smooth);
            bitField = SetBit(bitField, 6, info.Unicode);
            bitField = SetBit(bitField, 5, info.Italic);
            bitField = SetBit(bitField, 4, info.Bold);

            binaryWriter.Write(bitField);

            byte characterSetID = 0;

            if (!string.IsNullOrEmpty(info.Charset))
            {
                if (!Enum.TryParse(info.Charset, true, out CharacterSet characterSet))
                {
                    throw new FormatException("Invalid character set.");
                }

                characterSetID = (byte)characterSet;
            }

            binaryWriter.Write(characterSetID);

            binaryWriter.Write((ushort)info.StretchHeight);
            binaryWriter.Write((byte)info.SuperSamplingLevel);

            binaryWriter.Write((byte)info.PaddingUp);
            binaryWriter.Write((byte)info.PaddingRight);
            binaryWriter.Write((byte)info.PaddingDown);
            binaryWriter.Write((byte)info.PaddingLeft);

            binaryWriter.Write((byte)info.SpacingHorizontal);
            binaryWriter.Write((byte)info.SpacingVertical);

            binaryWriter.Write((byte)info.Outline);
            WriteNullTerminatedString(binaryWriter, info.FontName);
        }

        private void WriteCharacter(
            Character character,
            BinaryWriter binaryWriter,
            int id)
        {
            binaryWriter.Write((uint)id);
            binaryWriter.Write((ushort)character.X);
            binaryWriter.Write((ushort)character.Y);
            binaryWriter.Write((ushort)character.Width);
            binaryWriter.Write((ushort)character.Height);
            binaryWriter.Write((short)character.XOffset);
            binaryWriter.Write((short)character.YOffset);
            binaryWriter.Write((short)character.XAdvance);
            binaryWriter.Write((byte)character.Page);
            binaryWriter.Write((byte)character.Channel);
        }

        private void WriteKerningPair(
            KerningPair kerningPair,
            BinaryWriter binaryWriter,
            int amount)
        {
            binaryWriter.Write((uint)kerningPair.First);
            binaryWriter.Write((uint)kerningPair.Second);
            binaryWriter.Write((short)amount);
        }

        private static byte SetBit(
            byte @byte,
            int index,
            bool set)
        {
            if (index < 0 || index > 7)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (set)
            {
                return (byte)(@byte | (1 << index));
            }

            return (byte)(@byte & ~(1 << index));
        }

        private static void WriteNullTerminatedString(
            BinaryWriter binaryWriter,
            string value)
        {
            if (value != null)
            {
                foreach (var character in value)
                {
                    binaryWriter.Write((byte)character);
                }
            }

            binaryWriter.Write((byte)0);
        }
    }
}
