using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorRarity
{
    public enum StrengthLevel
    {
        Lowest,
        Low,
        Medium,
        High,
        VeryHigh,
        Maximum
    }

    public static List<Color> strengthColors = new List<Color>()
    {
        Color.gray,
        Color.green,
        Color.blue,
        new Color(0.6f, 0.2f, 1f), // Purple
        Color.red,
        Color.yellow
    };

    public static Color GetStrengthColor(StrengthLevel level)
    {
        switch (level)
        {
            case StrengthLevel.Lowest:
                return Color.gray;
            case StrengthLevel.Low:
                return Color.green;
            case StrengthLevel.Medium:
                return Color.blue;
            case StrengthLevel.High:
                return new Color(0.6f, 0.2f, 1f); // Purple
            case StrengthLevel.VeryHigh:
                return Color.red;
            case StrengthLevel.Maximum:
                return Color.yellow;
            default:
                return Color.white;
        }
    }

}

