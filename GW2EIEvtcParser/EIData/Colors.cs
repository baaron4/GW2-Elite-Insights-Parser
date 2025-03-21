namespace GW2EIEvtcParser.EIData;

public class Color
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
    public static Color Red = new(255, 0, 0);
    public static Color LightRed = new(255, 128, 128);
    public static Color DarkRed = new(128, 0, 0);
    public static Color RedBrownish = new(71, 35, 32);
    public static Color RedSkin = new(198, 101, 94);
    public static Color Orange = new(255, 100, 0);
    public static Color LightOrange = new(250, 120, 0);
    public static Color BreakbarRecoveringOrange = new(168, 104, 61);
    public static Color Sand = new(177, 157, 111);
    public static Color Yellow = new(255, 220, 0);
    public static Color LightBrown = new(196, 164, 132);
    public static Color Brown = new(120, 100, 0);
    public static Color Chocolate = new(123, 63, 0);
    public static Color Lime = new(200, 255, 100);
    public static Color Green = new(0, 255, 0);
    public static Color GreenishYellow = new(220, 255, 0);
    public static Color MilitaryGreen = new(69, 75, 27);
    public static Color LightMilitaryGreen = new(49, 71, 0);
    public static Color GreyishGreen = new(40, 57, 54);
    public static Color SligthlyDarkGreen = new(0, 180, 0);
    public static Color DarkGreen = new(0, 128, 0);
    public static Color DarkBlue = new(0, 0, 128);
    public static Color Teal = new(0, 255, 255);
    public static Color DarkTeal = new(0, 160, 150);
    public static Color LightBlue = new(0, 140, 255);
    public static Color BreakbarActiveBlue = new(75, 173, 168);
    public static Color DarkPurpleBlue = new(25, 25, 112);
    public static Color Purple = new(150, 0, 255);
    public static Color DarkPurple = new(50, 0, 150);
    public static Color LightPurple = new(200, 140, 255);
    public static Color Pink = new(255, 0, 150);
    public static Color LightPink = new(255, 140, 255);
    public static Color DarkPink = new(128, 0, 75);
    public static Color Magenta = new(255, 0, 255);
    public static Color DarkMagenta = new(128, 0, 128);
    public static Color Blue = new(0, 0, 255);
    public static Color CobaltBlue = new(0, 50, 180);
    public static Color SkyBlue = new(69, 182, 254);
    public static Color Ice = new(200, 233, 233);
    public static Color White = new(255, 255, 255);
    public static Color DarkWhite = new(180, 180, 180);
    public static Color Grey = new(60, 60, 60);
    public static Color LightGrey = new(120, 120, 120);
    public static Color BreakbarImmuneGrey = new(44, 45, 54);
    public static Color Black = new(0, 0, 0);

    public static Color Guardian = new(0x3399cc);
    public static Color Revenant = new(0xcc6342);
    public static Color Warrior = new(0xff9933);
    public static Color Engineer = new(0x996633);
    public static Color Ranger = new(0x66cc33);
    public static Color Thief = new(0xcc6666);
    public static Color Elementalist = new(0xec5752);
    public static Color Mesmer = new(0x993399);
    public static Color Necromancer = new(0x339966);
}
