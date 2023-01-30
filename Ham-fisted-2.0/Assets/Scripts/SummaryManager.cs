using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SummaryManager : MonoBehaviour
{
    public static SummaryManager instance { get; private set; }

    [SerializeField] private Image[] playerIcons;
    [SerializeField] private Transform[] kosContainers;
    [SerializeField] private Transform[] fallsContainers;
    [SerializeField] private GameObject statsIcon;
    [SerializeField] private Button homeButton;
    [SerializeField] private GameObject[] SummarySections;

    private StatTracker statTracker;

    private void Start()
    {
        instance = this;
        statTracker = StatTracker.instance;
        for (int x = 0; x < PlayerConfigManager.instance.playerConfigs.Count; x++)
        {
            playerIcons[x].color = GameManager.instance.colors[x];
            SummarySections[x].SetActive(true);
        }
        gameObject.SetActive(false);
    }

    [ContextMenu("Show Summary")]
    public void ShowSummary ()
    {
        gameObject.SetActive(true);
        homeButton.Select();
        if (statTracker.kos != null)
        {
            foreach (Vector2 pair in statTracker.kos)
            {
                SpawnStatsIcon((int)pair.x, (int)pair.y, "ko");
                SpawnStatsIcon((int)pair.y, (int)pair.x, "fell");
            }
        }
        if(statTracker.sds != null)
        {
            foreach (int player in statTracker.sds)
            {
                SpawnStatsIcon(player, player, "fell");
            }
        }
    }

    public void OnMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    private void SpawnStatsIcon (int player, int color, string type)
    {
        Transform targetTransform;
        if (type == "ko")
            targetTransform = kosContainers[player];
        else
            targetTransform = fallsContainers[player];
        StatsIcon icon = Instantiate(statsIcon, targetTransform).GetComponent<StatsIcon>();
        icon.ballTop.color = GameManager.instance.colors[color];
    }
}
