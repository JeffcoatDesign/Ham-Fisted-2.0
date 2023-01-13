using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;

public class StatsScreen : MonoBehaviourPun
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
        backButton.interactable = PhotonNetwork.IsMasterClient;
        Cursor.lockState = CursorLockMode.None;
        statTracker = StatTracker.instance;
        leaderboardName = GetLeaderBoardName(statTracker.gamemode, statTracker.stage);
        leaderboardTitle.text = statTracker.gamemode + " Most Kills";
        SetTimeText();
        SpawnDeaths();
        SpawnKOs();
        DeleteStatTracker();

        if (statTracker.isTimeBased)
            SendStat((int)statTracker.time);
        else
            SendStat(statTracker.killedPlayers.Length);

        photonView.RPC("RetrieveStats", RpcTarget.AllBuffered);

        Invoke("RetrieveStats", timeToRefreshStats);
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

    void SendStat (int statToSend)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest 
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = leaderboardName, Value = statToSend }
            }
        },
        result => { RetrieveStats(); },
        error => { Debug.Log(error.ErrorMessage); }
        );
    }

    [PunRPC]
    public void RetrieveStats ()
    {
        GetLeaderboardRequest getLeaderboardRequest = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            MaxResultsCount = numEntries
        };

        PlayFabClientAPI.GetLeaderboard(getLeaderboardRequest, 
            result => { UpdateLeaderboardUI(result.Leaderboard); }, 
            error => { Debug.Log(error.ErrorMessage); }
            );
    }

    void UpdateLeaderboardUI (List<PlayerLeaderboardEntry> leaderboard)
    {
        leaderboardText.text = "";
        for (int x = 0; x < leaderboard.Count; x++)
        {
            leaderboardText.text += (leaderboard[x].Position + 1) + ". " + leaderboard[x].DisplayName;
            leaderboardText.text += " - " + leaderboard[x].StatValue + "\n";
        }
    }

    void DeleteStatTracker ()
    {
        Destroy(statTracker.gameObject);
    }

    public void OnBackButton()
    {
        PhotonNetwork.OpRemoveCompleteCache();
        photonView.RPC("GoBackToMenu", RpcTarget.All);
    }

    [PunRPC]
    public void GoBackToMenu ()
    {
        NetworkManager.instance.ChangeScene("Menu");
    }
}
