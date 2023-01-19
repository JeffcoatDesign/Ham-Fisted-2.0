using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
//https://www.youtube.com/watch?v=_5pOiYHJgl0
public class PlayerConfigManager : MonoBehaviour
{
    public List<PlayerConfig> playerConfigs;
    [SerializeField] private int MaxPlayers = 4;
    public static PlayerConfigManager instance { get; private set; }
    public UnityEvent allReady;
    public Transform[] menuPositions;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
            playerConfigs = new List<PlayerConfig>();
        }
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += ChangeActiveScene;
        if (allReady == null)
            allReady = new UnityEvent();
    }

    void ChangeActiveScene(Scene current, Scene next)
    {
        string nextName = next.name;
        foreach(PlayerConfig pc in playerConfigs) {
            pc.Player.inGameScene = (nextName != "Menu");
            pc.Player.rig.isKinematic = (nextName == "Menu");
            if (nextName == "Menu")
                PositionInMenu(pc);
        }
    }

    public void ReadyPlayer (int index)
    {
        playerConfigs[index].IsReady = true;
        if(playerConfigs.Count == MaxPlayers && playerConfigs.All(p => p.IsReady == true))
        {
            allReady.Invoke();
        }
    }

    public void HandlePlayerJoin (PlayerInput pi)
    {
        GameObject playerGO = pi.transform.parent.gameObject;
        PlayerController player = playerGO.GetComponentInChildren<PlayerController>();
        playerGO.transform.SetParent(transform);
        if (!playerConfigs.Any(p => p.playerIndex == pi.playerIndex))
        {
            playerConfigs.Add(new PlayerConfig(pi, player));
        }
        if (SceneManager.GetActiveScene().name == "Menu")
            PositionInMenu(playerConfigs[pi.playerIndex]);
    }

    void PositionInMenu(PlayerConfig pc)
    {
        pc.Player.transform.position = menuPositions[pc.playerIndex].position;
    }
}

public class PlayerConfig
{
    public PlayerInput Input { get; set; }
    public PlayerController Player { get; set; }
    public int playerIndex { get; set; }
    public bool IsReady;

    public PlayerConfig(PlayerInput pi, PlayerController player)
    {
        playerIndex = pi.playerIndex;
        Player = player;
        Input = pi;
        Player.id = playerIndex;
        Player.Initialize(this);
    }
}
