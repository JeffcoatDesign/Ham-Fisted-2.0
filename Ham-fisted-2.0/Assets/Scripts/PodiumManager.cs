using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PodiumManager : MonoBehaviour
{
    [SerializeField] private GameObject statsIcon;
    [SerializeField] private GameObject podiumCamera;
    [SerializeField] private GameObject[] podiums;
    [SerializeField] private Transform[] podiumPositions;
    [SerializeField] private Transform[] koContainers;
    [SerializeField] private Transform[] fallsContainers;
    public static PodiumManager instance;

    private StatTracker statTracker;
    void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        statTracker = StatTracker.instance;
        foreach(GameObject podium in podiums)
        {
            podium.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    private void SpawnStatsIcon(int player, int color, string type)
    {
        if (player == -1)
            return;
        Transform targetTransform;
        if (type == "ko")
            targetTransform = koContainers[player];
        else
            targetTransform = fallsContainers[player];
        StatsIcon icon = Instantiate(statsIcon, targetTransform).GetComponent<StatsIcon>();
        icon.ballTop.color = GameManager.instance.colors[color];
    }

    [ContextMenu("ShowPodium")]
    public void ShowPodium ()
    {
        gameObject.SetActive(true);
        StartCoroutine(SummonPodium());
    }

    int GetRankIndex(PlayerController[] rankedPlayers, int index)
    {
        if (rankedPlayers.Any(p => p.id == index))
        {
            int rankIndex = 0;
            PlayerController player = rankedPlayers.First(p => p.id == index);
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

    IEnumerator SummonPodium ()
    {
        yield return null;
        PlayerController[] rankedPlayers = GameManager.instance.players;
        rankedPlayers = rankedPlayers.OrderByDescending(p => p.livesLeft).ThenByDescending(p => p.kills).ToArray();
        for (int i = 0; i < rankedPlayers.Length; i++)
        {
            podiums[i].SetActive(true);
            rankedPlayers[i].SetLocation(podiumPositions[i], true);
            //rankedPlayers[i].SetPosition(podiumPositions[i].transform);
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
