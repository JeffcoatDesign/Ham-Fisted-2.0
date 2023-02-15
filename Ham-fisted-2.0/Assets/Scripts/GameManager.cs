using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public string gamemode;
    public bool isTimeBased;
    public float gameTime;
    public float postGameTime;
    public bool playersRespawn;
    public int playerLives;
    public float fallY;
    public bool gameRunning = false;

    public SpawnPoint[] spawnPoints;

    public string playerPrefabLoc;
    public PlayerController[] players;
    public int alivePlayers;
    private int playersInGame;
    private float startTime;
    private float currentTime;
    public Color[] colors;

    private CameraManager cameraManager;

    public UnityEvent onGameEnd;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
        if (onGameEnd == null)
            onGameEnd = new UnityEvent();
    }

    private void Start()
    {
        cameraManager = gameObject.GetComponent<CameraManager>();
        players = new PlayerController[PlayerConfigManager.instance.playerConfigs.Count];
        foreach (PlayerConfig pc in PlayerConfigManager.instance.playerConfigs)
        {
            players[pc.playerIndex] = pc.Player;
            pc.Player.LateInitialize(pc);
            alivePlayers++;
            playersInGame++;
            cameraManager.playerGameUIs[pc.playerIndex].SpawnPlayerIcon(pc.playerIndex);
            SpawnPoint sp = spawnPoints.First(sp => !sp.isCollidingWithPlayer);
            pc.Player.SetLocation(sp.transform, false);
            sp.isCollidingWithPlayer = true;
            ShuffleSpawnPoints();
        }
        GUIManager.Instance.SetMinimap(alivePlayers);
        StatTracker.instance.SetGamemode();
        Cursor.lockState = CursorLockMode.Locked;
        cameraManager.Initiate(playersInGame);
        StartGame();
    }

    private void FixedUpdate()
    {
        if (!gameRunning)
            return;

        currentTime = Time.time - startTime;
        if (gameTime - currentTime <= 0)
            TimerOver();

        for (int i = 0; i < PlayerConfigManager.instance.playerConfigs.Count; i++)
            GUIManager.Instance.SetTimerText(gameTime - currentTime);
    }

    public PlayerController GetPlayer(int playerId)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.id == playerId)
                return player;
        }

        return null;
    }

    public PlayerController GetPlayer(GameObject playerObject)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.gameObject == playerObject)
                return player;
        }

        return null;
    }

    public void CheckWinCondition()
    {
        if (!gameRunning)
            return;
        if (alivePlayers == 1)
        {
            WinGame(players.First(p => !p.dead).id);
        }
        else if (alivePlayers < 1)
        {
            // Debug for testing on singleplayer Invoke("GoToStatsScreen", postGameTime);
            Invoke("GoBackToMenu", postGameTime);
            foreach (PlayerConfig pc in PlayerConfigManager.instance.playerConfigs)
                cameraManager.playerGameUIs[pc.playerIndex].SetWinText("No one");
        }
    }

    /*void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLoc, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.rotation);

        playerObj.GetComponentInChildren<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }*/

    public void StartGame ()
    {
        startTime = Time.time;
        gameRunning = true;
    }

    void TimerOver ()
    {
        gameRunning = false;
        PlayerController[] rankedPlayers = players;
        rankedPlayers = rankedPlayers.OrderByDescending(p => p.kills).ThenByDescending(p => p.livesLeft).ToArray();
        WinGame(rankedPlayers.First(x => !x.dead).id);
    }

    void WinGame(int winningPlayer)
    {
        onGameEnd.Invoke();
        gameRunning = false;
        PlayerConfigManager.instance.EnableControls("Menu");
        Cursor.lockState = CursorLockMode.Confined;
        // set the UI Win Text
        foreach (PlayerConfig pc in PlayerConfigManager.instance.playerConfigs)
            cameraManager.playerGameUIs[pc.playerIndex].SetWinText(GetPlayer(winningPlayer).nickname);
    }

    void GoBackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ShuffleSpawnPoints()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            SpawnPoint tempSP = spawnPoints[randomIndex];
            spawnPoints[randomIndex] = spawnPoints[i];
            spawnPoints[i] = tempSP;
        }
    }
}
