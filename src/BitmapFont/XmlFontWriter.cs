//**************************************************************************************************
// BitmapFont.cs                                                                                   *
// Copyright (c) 2018-2020 Aurora Berta-Oldham                                                     *
// This code is made available under the MIT License.                                              *
//**************************************************************************************************

using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace HotChai.Fonts.Bitmap
{
    internal sealed class XmlFontWriter
    {
        public void WriteFont(
            BitmapFont font,
            string path)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            };

            using (var stream = File.Create(path))
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    this.WriteFont(font, writer);
                }
            }
        }

        private void WriteFont(
            BitmapFont font,
            XmlWriter xmlWriter)
        {
            var document = new XDocument();


            var fontElement = new XElement("font");
            document.Add(fontElement);

            // Info

            if (font.Info != null)
            {
                var infoElement = new XElement("info");
                this.WriteInfo(font.Info, infoElement);
                fontElement.Add(infoElement);
            }

            // Common

            if (font.Common != null)
            {
                var commonElement = new XElement("common");
                this.WriteCommon(font.Common, commonElement, font.Pages.Count);
                fontElement.Add(commonElement);
            }

            // Pages

            if (font.Pages != null)
            {
                var pagesElement = new XElement("pages");

                foreach (var page in font.Pages)
                {
                    var pageElement = new XElement("page");
                    pageElement.SetAttributeValue("id", page.Key);
                    pageElement.SetAttributeValue("file", page.Value);
                    pagesElement.Add(pageElement);
                }

                fontElement.Add(pagesElement);
            }

            // Characters

            if (font.Characters != null)
            {
                var charactersElement = new XElement("chars");
                charactersElement.SetAttributeValue("count", font.Characters.Count);

                foreach (var keyValuePair in font.Characters)
                {
                    var characterElement = new XElement("char");
                    WriteCharacter(keyValuePair.Value, characterElement, keyValuePair.Key);
                    charactersElement.Add(characterElement);
                }

                fontElement.Add(charactersElement);
            }

            // Kernings

            if (font.KerningPairs != null && font.KerningPairs.Count > 0)
            {
                var kerningsElement = new XElement("kernings");
                kerningsElement.SetAttributeValue("count", font.KerningPairs.Count);

                foreach (var keyValuePair in font.KerningPairs)
                {
                    var kerningElement = new XElement("kerning");
                    WriteKerningPair(keyValuePair.Key, kerningElement, keyValuePair.Value);
                    kerningsElement.Add(kerningElement);
                }

                fontElement.Add(kerningsElement);
            }

            document.WriteTo(xmlWriter);
        }

        private void WriteCommon(
            BitmapFontCommon common,
            XElement element,
            int pages)
        {
            element.SetAttributeValue("lineHeight", common.LineHeight);
            element.SetAttributeValue("base", common.Base);
            element.SetAttributeValue("scaleW", common.ScaleWidth);
            element.SetAttributeValue("scaleH", common.ScaleHeight);

            element.SetAttributeValue("pages", pages);

            element.SetAttributeValue("packed", Convert.ToInt32(common.Packed));

            element.SetAttributeValue("alphaChnl", (int)common.AlphaChannel);
            element.SetAttributeValue("redChnl", (int)common.RedChannel);
            element.SetAttributeValue("greenChnl", (int)common.GreenChannel);
            element.SetAttributeValue("blueChnl", (int)common.BlueChannel);
        }

        private void WriteInfo(
            BitmapFontInfo info,
            XElement element)
        {
            element.SetAttributeValue("face", info.FontName ?? string.Empty);
            element.SetAttributeValue("size", info.Size);
            element.SetAttributeValue("bold", Convert.ToInt32(info.Bold));
            element.SetAttributeValue("italic", Convert.ToInt32(info.Italic));

            element.SetAttributeValue("charset", info.Charset ?? string.Empty);

            element.SetAttributeValue("unicode", Convert.ToInt32(info.Unicode));
            element.SetAttributeValue("stretchH", info.StretchHeight);
            element.SetAttributeValue("smooth", Convert.ToInt32(info.Smooth));
            element.SetAttributeValue("aa", info.SuperSamplingLevel);

            var padding = $"{info.PaddingUp},{info.PaddingRight},{info.PaddingDown},{info.PaddingLeft}";
            element.SetAttributeValue("padding", padding);

            var spacing = $"{info.SpacingHorizontal},{info.SpacingVertical}";
            element.SetAttributeValue("spacing", spacing);

            element.SetAttributeValue("outline", info.Outline);
        }

        private void WriteCharacter(
            Character character,
            XElement element,
            int id)
        {
            element.SetAttributeValue("id", id);
            element.SetAttributeValue("x", character.X);
            element.SetAttributeValue("y", character.Y);
            element.SetAttributeValue("width", character.Width);
            element.SetAttributeValue("height", character.Height);
            element.SetAttributeValue("xoffset", character.XOffset);
            element.SetAttributeValue("yoffset", character.YOffset);
            element.SetAttributeValue("xadvance", character.XAdvance);
            element.SetAttributeValue("page", character.Page);
            element.SetAttributeValue("chnl", (int)character.Channel);
        }

        private void WriteKerningPair(
            KerningPair kerningPair,
            XElement element,
            int amount)
        {
            element.SetAttributeValue("first", kerningPair.First);
            element.SetAttributeValue("second", kerningPair.Second);
            element.SetAttributeValue("amount", amount);
        }
    }
}
