using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;


[System.Serializable] public class DisplayWinnersEvent : UnityEvent<string>{}

public class PodiumManager : MonoBehaviour
{
    [SerializeField] private GameObject statsIcon;
    [SerializeField] private GameObject podiumCamera;
    [SerializeField] private GameObject[] podiumPrefabs;
    [SerializeField] private Transform[] podiumSpawnPositions;
    public DisplayWinnersEvent WinnersEvent;
    public UnityEvent OnPodiumAppear; 
    public static PodiumManager instance;

    private StatTracker statTracker;
    private Podium[] podiums;
    void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        statTracker = StatTracker.instance;
        podiumCamera.SetActive(false);
        gameObject.SetActive(false);
        if (OnPodiumAppear == null)
            OnPodiumAppear = new();
        if (WinnersEvent == null)
            WinnersEvent = new();
        GameManager.instance.onGameEnd.AddListener(ShowPodium);
    }

    private void SpawnStatsIcon(int player, int color, string type)
    {
        if (player == -1)
            return;
        Transform targetTransform;
        if (type == "ko")
            targetTransform = podiums[player].koContainer;
        else
            targetTransform = podiums[player].livesContainer;
        StatsIcon icon = Instantiate(statsIcon, targetTransform).GetComponent<StatsIcon>();
        icon.ballTop.color = GameManager.instance.colors[color];
    }

    [ContextMenu("ShowPodium")]
    public void ShowPodium ()
    {
        gameObject.SetActive(true);
        StartCoroutine(SummonPodium());
    }

    int GetRankIndex(RankedPlayer[] rankedPlayers, int index)
    {
        if (rankedPlayers.Any(p => p.Player.id == index))
        {
            int rankIndex = 0;
            RankedPlayer player = rankedPlayers.First(p => p.Player.id == index);
            for (int i = 0; i < rankedPlayers.Length; i++)
            {
                if (player == rankedPlayers[i])
                    rankIndex = i;
            }
            return rankIndex;
        }
        else
            return -1;
    }

    void FindTiedPlayers (RankedPlayer[] rankedPlayers)
    {
        for (int i = 0; i < rankedPlayers.Length; i++)
        {
            if (i + 1 < rankedPlayers.Length && rankedPlayers[i].Rank != rankedPlayers[i + 1].Rank &&rankedPlayers[i].Rank < i + 1) rankedPlayers[i + 1].Rank -= 1;
            foreach (RankedPlayer rankedPlayer in rankedPlayers)
            {
                if (rankedPlayer.Player.kills == rankedPlayers[i].Player.kills && rankedPlayer.Player.livesLeft == rankedPlayers[i].Player.livesLeft)
                    rankedPlayer.Rank = rankedPlayers[i].Rank;
            }
        }
        foreach (RankedPlayer rankedPlayer in rankedPlayers)
        {
            if (rankedPlayer.Player.dead)
                rankedPlayer.Rank = rankedPlayer.Player.deathRank;
        }
    }
    string WinList(RankedPlayer[] rankedPlayers)
    {
        List<RankedPlayer> firstPlacePlayers = new();
        foreach(RankedPlayer rankedPlayer in rankedPlayers)
        {
            if (rankedPlayer.Rank == 1) firstPlacePlayers.Add(rankedPlayer);
        }
        if (firstPlacePlayers.Count != 1)
        {
            string firstList = null;
            for (int i = 0; i < firstPlacePlayers.Count; i++)
            {
                if (i != 0) 
                {
                    if (firstPlacePlayers.Count != 2) firstList += ",";
                    firstList += " "; 
                }
                if (i == firstPlacePlayers.Count - 1) firstList += "and ";
                firstList += firstPlacePlayers.ToArray()[i].Player.nickname;
            }
            firstList += " are the Champs!";
            return firstList;
        }
        return rankedPlayers[0].Player.nickname + " is the Champion!";
    }

    IEnumerator SummonPodium ()
    {
        yield return null;
        RankedPlayer[] rankedPlayers = new RankedPlayer[GameManager.instance.players.Length];
        podiums = new Podium[GameManager.instance.players.Length];
        foreach (PlayerController playerController in GameManager.instance.players)
        {
            rankedPlayers[playerController.id] = new(playerController);
        }

        rankedPlayers = rankedPlayers.OrderByDescending(p => p.Player.livesLeft).ThenByDescending(p => p.Player.kills).ToArray();
        for (int i = 0; i < rankedPlayers.Length; i++)
        {
            rankedPlayers[i].Rank = i + 1;
            rankedPlayers[i].PodiumIndex = i;
            //rankedPlayers[i].SetPosition(podiumPositions[i].transform);
        }
        FindTiedPlayers(rankedPlayers);
        WinnersEvent.Invoke(WinList(rankedPlayers));
        yield return new WaitForSeconds(3f);
        OnPodiumAppear.Invoke();
        foreach(RankedPlayer rankedPlayer in rankedPlayers)
        {
            podiums[rankedPlayer.PodiumIndex] = Instantiate(podiumPrefabs[rankedPlayer.Rank - 1], podiumSpawnPositions[rankedPlayer.PodiumIndex]).GetComponent<Podium>();
            rankedPlayer.Player.SetLocation(podiums[rankedPlayer.PodiumIndex].podiumPosition, true);
            rankedPlayer.Player.PodiumAnimation(rankedPlayer.Rank);
            rankedPlayer.Player.ToggleGlove(false);
        }
        if (statTracker.kos != null)
        {
            foreach (Vector2 pair in statTracker.kos)
            {
                SpawnStatsIcon(GetRankIndex(rankedPlayers, (int)pair.x), (int)pair.y, "ko");
                SpawnStatsIcon(GetRankIndex(rankedPlayers, (int)pair.y), (int)pair.x, "fell");
            }
        }
        if (statTracker.sds != null)
        {
            foreach (int player in statTracker.sds)
            {
                SpawnStatsIcon(GetRankIndex(rankedPlayers, player), player, "fell");
            }
        }
        podiumCamera.SetActive(true);
        yield return null;
    }
}

public class RankedPlayer 
{
    public PlayerController Player;
    public int Rank;
    public int PodiumIndex;

    public RankedPlayer (PlayerController player)
    {
        Player = player;
        Rank = player.id + 1;
        PodiumIndex = player.id;
    }
}
