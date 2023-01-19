using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ShuffleSpawnPoints();
        players = new PlayerController[PlayerConfigManager.instance.playerConfigs.Count];
        foreach (PlayerConfig pc in PlayerConfigManager.instance.playerConfigs)
        {
            players[pc.playerIndex] = pc.Player;
            pc.Player.Initialize(pc);
            alivePlayers++;
            playersInGame++;
            GameUI.instance.SpawnPlayerIcon(pc.playerIndex);
            pc.Player.transform.position = spawnPoints[pc.playerIndex].transform.position;
        }
        cameraManager = gameObject.GetComponent<CameraManager>();
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
        GameUI.instance.SetTimerText(gameTime - currentTime);
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
        if (alivePlayers == 1)
            WinGame(players.First(x => !x.dead).id);
        else if (alivePlayers < 1)
        {
            // Debug for testing on singleplayer Invoke("GoToStatsScreen", postGameTime);
            Invoke("GoBackToMenu", postGameTime);
            GameUI.instance.SetWinText("No one");
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
        players = players.OrderBy(p => p.kills).ThenBy(p => p.kills).ToArray();
        WinGame(players.First(x => !x.dead).id);
    }

    void WinGame(int winningPlayer)
    {
        // set the UI Win Text
        GameUI.instance.SetWinText(GetPlayer(winningPlayer).nickname);

        Invoke("GoBackToMenu", postGameTime);
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