using UnityEngine;

public static class TwitchColors
{
    private static Color FromHex(int i)
    {
        float r = (i >> 16) & 255;
        float g = (i >> 8) & 255;
        float b = i & 255;
        return new Color(r / 255f, g / 255f, b / 255f, 1);
    }

    public static Color Blue = new Color(0.2216981f, 0.2216981f, 1f, 1f);
    public static Color Coral = FromHex(0xFF7F50);
    public static Color DodgerBlue = FromHex(0x1E90FF);
    public static Color SpringGreen = FromHex(0x00FA9A);
    public static Color YellowGreen = FromHex(0x9ACD32);
    public static Color Green = FromHex(0x00FF00);
    public static Color OrangeRed = FromHex(0xFF4500);
    public static Color Red = FromHex(0xFF0000);
    public static Color GoldenRod = FromHex(0xDAA520);
    public static Color HotPink = FromHex(0xFF69B4);
    public static Color CadetBlue = FromHex(0x5F9EA0);
    public static Color SeaGreen = FromHex(0x2E8B57);
    public static Color Chocolate = FromHex(0xD2691E);
    public static Color BlueViolet = FromHex(0x8A2BE2);
    public static Color Firebrick = FromHex(0xB22222);
    public static Color[] All = {
        Blue,
        Coral,
        DodgerBlue,
        SpringGreen,
        YellowGreen,
        Green,
        OrangeRed,
        Red,
        GoldenRod,
        HotPink,
        CadetBlue,
        SeaGreen,
        Chocolate,
        BlueViolet,
        Firebrick
    };
}