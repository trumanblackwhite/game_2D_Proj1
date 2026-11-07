using UnityEngine;

public static class HpColors
{
    private static readonly Color High = Hex("3ED24E");   // hijau > 50%
    private static readonly Color Mid  = Hex("FFC61A");   // emas > 20% (sama dengan UiTheme.Gold)
    private static readonly Color Low  = Hex("E23B3B");   // merah selebihnya

    public static Color For(float ratio)
    {
        if (ratio > 0.5f) return High;
        if (ratio > 0.2f) return Mid;
        return Low;
    }

    private static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color color);
        return color;
    }
}