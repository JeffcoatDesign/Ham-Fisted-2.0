using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
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

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StatTracker.instance.SetGamemode();
        Cursor.lockState = CursorLockMode.Locked;
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    private void FixedUpdate()
    {
        if (!gameRunning)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            currentTime = Time.time - startTime;
            if (gameTime - currentTime <= 0)
                photonView.RPC("TimerOver", RpcTarget.All);
        }
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
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
        else if (alivePlayers < 1)
        {
            // Debug for testing on singleplayer Invoke("GoToStatsScreen", postGameTime);
            Invoke("GoBackToMenu", postGameTime);
            GameUI.instance.SetWinText("No one");
        }
    }

    [PunRPC]
    public void ImInGame ()
    {
        playersInGame++;

        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("SpawnPlayer", RpcTarget.All);
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    [PunRPC]
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLoc, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.rotation);

        playerObj.GetComponentInChildren<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void StartGame ()
    {
        startTime = Time.time;
        gameRunning = true;
    }

    [PunRPC]
    void TimerOver ()
    {
        gameRunning = false;
        if (PhotonNetwork.IsMasterClient)
        {
            //find heighest elims
            players = players.OrderBy(p => p.kills).ThenBy(p => p.kills).ToArray();

            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
        }
    }

    [PunRPC]
    void WinGame(int winningPlayer)
    {
        // set the UI Win Text
        GameUI.instance.SetWinText(GetPlayer(winningPlayer).photonPlayer.NickName);

        Invoke("GoToStatsScreen", postGameTime);
    }

    void GoToStatsScreen()
    {
        StatTracker.instance.time = currentTime;

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.RemoveBufferedRPCs();

        NetworkManager.instance.ChangeScene("StatsScreen");
    }

    void GoBackToMenu()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.RemoveBufferedRPCs();

        NetworkManager.instance.ChangeScene("Menu");
    }

    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
    {
        if (!gameRunning)
            return;
        if (stream.IsWriting)
        {
            stream.SendNext(currentTime);
        }
        else if (stream.IsReading && !PhotonNetwork.IsMasterClient)
        {
            currentTime = (float)stream.ReceiveNext();
        }
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
