using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public Transform row;
    public GameObject playerIconPrefab;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI timerText;

    public static GameUI instance;

    private PlayerIcon[] icons = new PlayerIcon[12];

    private void Awake()
    {
        instance = this;
    }

    public void SpawnPlayerIcon (int index)
    {
        PlayerIcon icon = Instantiate(playerIconPrefab, row).GetComponentInChildren<PlayerIcon>();
        icon.SetNumber(index);
        icon.SetColor(index);
        icon.SpawnLives(GameManager.instance.playerLives);
        icons[index] = icon;
    }

    public void RemoveLife(int index)
    {
        icons[index].RemoveLife();
    }

    public void RemoveIcon (int index)
    {
        icons[index].Remove();
    }

    public void SetWinText (string text)
    {
        winText.gameObject.SetActive(true);
        winText.text = text + " is the Champion";
    }

    public void SetTimerText (float time)
    {
        float minutes = Mathf.Floor(time / 60);
        float seconds = Mathf.RoundToInt(time % 60);
        if (seconds == 60)
        {
            seconds = 0;
            minutes++;
        }
        timerText.text = minutes.ToString("F0") + ":" + seconds.ToString("00");
    }
}
