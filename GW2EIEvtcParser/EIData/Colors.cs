using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class Color
    {
        private readonly int _r;
        private readonly int _g;
        private readonly int _b;
        private readonly float _a;

        public Color(int r, int g, int b, float a = 1.0f)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public string ToString(bool withAlpha)
        {
            if (withAlpha)
            {
                return "rgba(" + _r + ", " + _g + ", " + _b + ", " + _a + "}";
            }
            return "rgb(" + _r + ", " + _g + ", " + _b + "}";
        }
    }

    internal static class Colors
    {
        public static Color Red = new Color(255, 0, 0);
        public static Color DarkRed = new Color(128, 0, 0);
        public static Color Orange = new Color(255, 100, 0);
        public static Color LightOrange = new Color(255, 160, 0);
        public static Color Yellow = new Color(255, 220, 0);
        public static Color Brown = new Color(120, 100, 0);
        public static Color Green = new Color(0, 255, 0);
        public static Color DarkGreen = new Color(0, 128, 0);
        public static Color Teal = new Color(0, 255, 255);
        public static Color DarkTeal = new Color(0, 160, 150);
        public static Color LightBlue = new Color(0, 140, 255);
        public static Color Purple = new Color(150, 0, 255);
        public static Color DarkPurple = new Color(50, 0, 150);
        public static Color LightPurple = new Color(200, 140, 255);
        public static Color Pink = new Color(255, 0, 150);
        public static Color DarkPink = new Color(128, 0, 75);
        public static Color Magenta = new Color(255, 0, 255);
        public static Color DarkMagenta = new Color(128, 0, 128);
        public static Color Blue = new Color(0, 0, 255);
        public static Color White = new Color(255, 255, 255);
        public static Color Grey = new Color(60, 60, 60);
        public static Color LightGrey = new Color(120, 120, 120);
        public static Color Black = new Color(0, 0, 0);
        public static Color LightRed = new Color(255, 128, 128);
    }
}
