using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerConfigManager : MonoBehaviour
{
    public PlayerInputManager pIM;
    public List<PlayerConfig> playerConfigs;
    [SerializeField] private int MaxPlayers = 4;
    public static PlayerConfigManager instance { get; private set; }
    public UnityEvent allReady;
    public UnityEvent firstPlayerJoined;
    public UnityEvent playerJoined;
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
        if (firstPlayerJoined == null)
            firstPlayerJoined = new UnityEvent();
        if (playerJoined == null)
            playerJoined = new UnityEvent();
    }

    void ChangeActiveScene(Scene current, Scene next)
    {
        string nextName = next.name;
        pIM.EnableJoining();
        if (nextName != "Menu")
            pIM.DisableJoining();
        foreach(PlayerConfig pc in playerConfigs) {
            pc.Input.SwitchCurrentActionMap("Game");
            pc.Player.inGameScene = (nextName != "Menu");
            pc.Player.rig.velocity = Vector3.zero;
            if (nextName == "Menu")
            {
                PositionInMenu(pc);
                if (pc.playerIndex == 0)
                    pc.Input.SwitchCurrentActionMap("Menu");
            }
        }
    }

    public void EnableControls(string scheme)
    {
        foreach (PlayerConfig pc in playerConfigs)
        {
            pc.Input.SwitchCurrentActionMap(scheme);
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
        playerJoined.Invoke();
        if (playerConfigs.Count < 1)
            firstPlayerJoined.Invoke();
        GameObject playerGO = pi.transform.parent.gameObject;
        PlayerController player = playerGO.GetComponentInChildren<PlayerController>();
        playerGO.transform.SetParent(transform);
        if (!playerConfigs.Any(p => p.playerIndex == pi.playerIndex))
        {
            playerConfigs.Add(new PlayerConfig(pi, player));
        }
        if (SceneManager.GetActiveScene().name == "Menu") { 
            PositionInMenu(playerConfigs[pi.playerIndex]);
            if(pi.playerIndex != 0)
                pi.SwitchCurrentActionMap("Game");
        }
    }

    [ContextMenu("Spawn Debug Character")]
    void SpawnDebugPlayer ()
    {
        InputSystem.AddDevice<Gamepad>();
        pIM.JoinPlayer();
    }

    void PositionInMenu(PlayerConfig pc)
    {
        pc.Player.SetLocation(menuPositions[pc.playerIndex].transform, true);
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
