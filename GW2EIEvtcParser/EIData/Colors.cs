using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class Color
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public float A { get; set; }

        public Color(int r, int g, int b, float a = 1.0f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>Creates a new color with adjusted alpha value</summary>
        public Color WithAlpha(float alpha)
        {
            return new Color(R, G, B, alpha);
        }

        public string ToString(bool withAlpha = true)
        {
            return withAlpha ? $"rgba({R}, {G}, {B}, {A})" : $"rgb({R}, {G}, {B})";
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
