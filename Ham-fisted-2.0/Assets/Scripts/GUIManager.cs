using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private Transform minimap;
    [SerializeField] private Transform singleplayerAnchor;
    [SerializeField] private Transform multiplayerAnchor;
    [SerializeField] private TextMeshProUGUI timerText;

    public static GUIManager instance { get; private set; }
    public void Awake()
    {
        instance = this;
    }

    public void SetMinimap(int numPlayers)
    {
        if (numPlayers > 1)
            minimap.SetParent(multiplayerAnchor);
        else
            minimap.SetParent(singleplayerAnchor);
    }
    public void SetTimerText(float time)
    {
        float minutes = Mathf.Floor(time / 60);
        float seconds = Mathf.RoundToInt(time % 60);
        if (seconds == 60)
        {
            seconds = 0;
            minutes++;
        }
        if (minutes < 0)
        {
            minutes = 0;
            seconds = 0;
        }
        timerText.text = minutes.ToString("F0") + ":" + seconds.ToString("00");
    }

    public void HideGUI() 
    {
        minimap.gameObject.SetActive(false);
    }
}
