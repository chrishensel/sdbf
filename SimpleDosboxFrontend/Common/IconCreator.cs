using System.Collections.Generic;
using System.Drawing;
using SimpleDosboxFrontend.Data;

namespace SimpleDosboxFrontend.Common
{
    static class IconCreator
    {
        private static readonly StringFormat _stringFormat;

        static IconCreator()
        {
            _stringFormat = new StringFormat(StringFormat.GenericDefault)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
        }

        internal static Image CreateGenericImage(Profile profile)
        {
            var image = new Bitmap(32, 32);

            using (var graphics = Graphics.FromImage(image))
            {
                var backgroundColor = GetBackgroundColor(profile);

                using (var brush = new SolidBrush(backgroundColor))
                {
                    graphics.FillRectangle(brush, 0f, 0f, 64f, 64f);
                }

                var abbrev = GetAbbreviation(profile);

                using (var font = FindSuitableFont(image, graphics, abbrev))
                {
                    var unit = GraphicsUnit.Pixel;

                    graphics.DrawString(abbrev, font, Brushes.Black, image.GetBounds(ref unit), _stringFormat);
                }
            }

            return image;
        }

        private static Font FindSuitableFont(Bitmap image, Graphics graphics, string abbrev)
        {
            Font found = null;
            var currentSize = 64f;

            while (found == null)
            {
                var font = new Font(SystemFonts.DefaultFont.FontFamily, currentSize, FontStyle.Bold, GraphicsUnit.Pixel);
                var size = graphics.MeasureString(abbrev, font, image.Width, _stringFormat);

                if (size.Width < image.Width && size.Height < image.Height)
                {
                    found = font;
                }
                else
                {
                    currentSize--;
                }
            }

            return found;
        }

        private static Color GetBackgroundColor(Profile profile)
        {
            var backgroundColorHash = profile.Name.GetHashCode();
            var r = (byte)(backgroundColorHash >> 24);
            var g = (byte)(backgroundColorHash >> 16);
            var b = (byte)(backgroundColorHash >> 8);

            var backgroundColor = Color.FromArgb(r, g, b);
            return backgroundColor;
        }

        private static string GetAbbreviation(Profile profile)
        {
            var chars = new List<char>();
            var upperChars = 0;
            var ignoreNextChar = false;

            for (int i = 0; i < profile.Name.Length; i++)
            {
                var c = profile.Name[i];

                if ((char.IsUpper(c) || char.IsNumber(c)) && !ignoreNextChar)
                {
                    upperChars++;
                    ignoreNextChar = true;

                    if (chars.Count == 3)
                    {
                        chars[2] = c;
                    }
                    else
                    {
                        chars.Add(c);
                    }
                }

                if (char.IsSeparator(c))
                {
                    ignoreNextChar = false;
                }
            }

            return new string(chars.ToArray());
        }
    }
}