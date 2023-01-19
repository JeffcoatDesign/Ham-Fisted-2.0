using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsIcon : MonoBehaviour
{
    public Image ballTop;

    public void SetColor (Color color)
    {
        ballTop.color = color;
    }
}
