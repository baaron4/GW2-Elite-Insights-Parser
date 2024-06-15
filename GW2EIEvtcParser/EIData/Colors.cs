namespace GW2EIEvtcParser.EIData
{
    internal class Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public float A { get; set; }

        public Color(byte r, byte g, byte b, float a = 1.0f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(int rgb, float a = 1.0f)
        {
            R = (byte)(rgb >> 16);
            G = (byte)(rgb >> 8);
            B = (byte)rgb;
            A = a;
        }

        /// <summary>Creates a new color with adjusted alpha value</summary>
        public Color WithAlpha(float alpha)
        {
            return new Color(R, G, B, alpha);
        }

        /// <summary>Creates a new color with adjusted alpha value</summary>
        public Color WithAlpha(double alpha)
        {
            return WithAlpha((float)alpha);
        }

        public string ToString(bool withAlpha = true)
        {
            return withAlpha ? $"rgba({R}, {G}, {B}, {A})" : $"rgb({R}, {G}, {B})";
        }
    }

    internal static class Colors
    {
        public static Color Red = new Color(255, 0, 0);
        public static Color LightRed = new Color(255, 128, 128);
        public static Color DarkRed = new Color(128, 0, 0);
        public static Color RedBrownish = new Color(71, 35, 32);
        public static Color RedSkin = new Color(198, 101, 94);
        public static Color Orange = new Color(255, 100, 0);
        public static Color LightOrange = new Color(250, 120, 0);
        public static Color Yellow = new Color(255, 220, 0);
        public static Color LightBrown = new Color(196, 164, 132);
        public static Color Brown = new Color(120, 100, 0);
        public static Color Chocolate = new Color(123, 63, 0);
        public static Color Green = new Color(0, 255, 0);
        public static Color GreenishYellow = new Color(220, 255, 0);
        public static Color MilitaryGreen = new Color(69, 75, 27);
        public static Color LightMilitaryGreen = new Color(49, 71, 0);
        public static Color GreyishGreen = new Color(40, 57, 54);
        public static Color DarkGreen = new Color(0, 128, 0);
        public static Color DarkBlue = new Color(0, 0, 128);
        public static Color Teal = new Color(0, 255, 255);
        public static Color DarkTeal = new Color(0, 160, 150);
        public static Color LightBlue = new Color(0, 140, 255);
        public static Color Purple = new Color(150, 0, 255);
        public static Color DarkPurple = new Color(50, 0, 150);
        public static Color LightPurple = new Color(200, 140, 255);
        public static Color Pink = new Color(255, 0, 150);
        public static Color LightPink = new Color(255, 140, 255);
        public static Color DarkPink = new Color(128, 0, 75);
        public static Color Magenta = new Color(255, 0, 255);
        public static Color DarkMagenta = new Color(128, 0, 128);
        public static Color Blue = new Color(0, 0, 255);
        public static Color CobaltBlue = new Color(0, 50, 180);
        public static Color SkyBlue = new Color(69, 182, 254);
        public static Color Ice = new Color(200, 233, 233);
        public static Color White = new Color(255, 255, 255);
        public static Color DarkWhite = new Color(180, 180, 180);
        public static Color Grey = new Color(60, 60, 60);
        public static Color LightGrey = new Color(120, 120, 120);
        public static Color Black = new Color(0, 0, 0);

        public static Color Guardian = new Color(0x3399cc);
        public static Color Revenant = new Color(0xcc6342);
        public static Color Warrior = new Color(0xff9933);
        public static Color Engineer = new Color(0x996633);
        public static Color Ranger = new Color(0x66cc33);
        public static Color Thief = new Color(0xcc6666);
        public static Color Elementalist = new Color(0xec5752);
        public static Color Mesmer = new Color(0x993399);
        public static Color Necromancer = new Color(0x339966);
    }
}
