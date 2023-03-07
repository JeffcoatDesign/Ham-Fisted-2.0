using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;
    public Color[] colors;
    private Color[] defaultColors;
    public Material[] playerSkins;
    void Awake()
    {
        instance = this;
        defaultColors = colors;
    }

    public void ResetColors()
    {
        colors = defaultColors;
    }
}
