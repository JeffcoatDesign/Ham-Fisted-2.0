using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StatsScreen : MonoBehaviour
{
    public float timeToRefreshStats;

    [Header("Local Stats")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI kosLabel;
    public Transform kosContainer;
    public TextMeshProUGUI deathsLabel;
    public Transform deathsContainer;

    [Header("Leaderboard")]
    public TextMeshProUGUI leaderboardTitle;
    public TextMeshProUGUI leaderboardText;
    public int numEntries;

    [Header("Icons")]
    public GameObject statsIcon;
    public Color[] colors;

    [Header("Buttons")]
    public Button backButton;

    private StatTracker statTracker;
    private string leaderboardName;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        statTracker = StatTracker.instance;
        leaderboardName = GetLeaderBoardName(statTracker.gamemode, statTracker.stage);
        leaderboardTitle.text = statTracker.gamemode + " Most Kills";
        SetTimeText();
        SpawnDeaths();
        SpawnKOs();
        DeleteStatTracker();
    }

    void SetTimeText ()
    {
        float minutes = Mathf.Floor(statTracker.time / 60);
        float seconds = Mathf.RoundToInt(statTracker.time % 60);
        if (seconds == 60)
        {
            seconds = 0;
            minutes++;
        }
        timeText.text = "Time: " + minutes.ToString("F0") + ":" + seconds.ToString("00");
    }

    void SpawnKOs ()
    {
        foreach (int ko in statTracker.killedPlayers)
        {
            StatsIcon icon = Instantiate(statsIcon, kosContainer).GetComponent<StatsIcon>();
            icon.SetColor(colors[ko]);
        }
    }

    void SpawnDeaths()
    {
        foreach (int fallout in statTracker.killedMe)
        {
            StatsIcon icon = Instantiate(statsIcon, deathsContainer).GetComponent<StatsIcon>();
            icon.SetColor(colors[fallout]);
        }
    }

    string GetLeaderBoardName (string gamemode, string stage)
    {
        return stage + "-" + gamemode;
    }

    /*void UpdateLeaderboardUI (List<PlayerLeaderboardEntry> leaderboard)
    {
        leaderboardText.text = "";
        for (int x = 0; x < leaderboard.Count; x++)
        {
            leaderboardText.text += (leaderboard[x].Position + 1) + ". " + leaderboard[x].DisplayName;
            leaderboardText.text += " - " + leaderboard[x].StatValue + "\n";
        }
    }*/

    void DeleteStatTracker ()
    {
        Destroy(statTracker.gameObject);
    }

    public void OnBackButton()
    {
        GoBackToMenu();
    }

    public void GoBackToMenu ()
    {
        SceneManager.LoadScene("Menu");
    }
}
