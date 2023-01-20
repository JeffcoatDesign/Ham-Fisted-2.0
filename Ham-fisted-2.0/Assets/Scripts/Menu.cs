using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    #region instance
    public static Menu instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Controller")]
    public GameObject pCM;

    [Header("Game Selections")]
    public string selectedStage;

    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;
    public GameObject tutorialScreen;
    public GameObject creditsScreen;
    public GameObject startScreen;

    [Header("Main Screen")]
    public Button startButton;

    [Header("Lobby")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;

    [Header("Tutorial Screen")]
    public Button tutorialBackButton;

    [Header("Credits Screen")]
    public Button creditsBackButton;

    void Start ()
    {
        if (PlayerConfigManager.instance == null)
        {
            PlayerConfigManager cM = Instantiate(pCM).GetComponent<PlayerConfigManager>();
            cM.firstPlayerJoined.AddListener(OnPlayerOneJoined);
            SetScreen(startScreen);
        }
        else SetScreen(lobbyScreen);
        //enable the cursor
        Cursor.lockState = CursorLockMode.None;

        //are we in a game
        //SetScreen(lobbyScreen);
        //UpdateLobbyUI();
    }

    void SetScreen (GameObject screen)
    {
        //disable all other screens
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        tutorialScreen.SetActive(false);
        creditsScreen.SetActive(false);
        startScreen.SetActive(false);

        //activate requested screen
        screen.SetActive(true);

        if(screen == lobbyScreen)
            startGameButton.Select();
        if (screen == mainScreen)
            startButton.Select();
        if (screen == tutorialScreen)
            tutorialBackButton.Select();
        if (screen == creditsScreen)
            creditsBackButton.Select();
    }

    public void OnPlayerOneJoined ()
    {
        StartCoroutine("WaitToStart");
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForEndOfFrame();
        SetScreen(mainScreen);
    }

    public void OnBackButton ()
    {
        SetScreen(mainScreen);
    }
    // LOGIN SCREEN
    public void OnLogin()
    {
        SetScreen(mainScreen);
    }

    public void OnStartButton ()
    {
        SetScreen(lobbyScreen);
    }

    public void OnTutorialButton ()
    {
        SetScreen(tutorialScreen);
    }

    public void OnCreditsButton ()
    {
        SetScreen(creditsScreen);
    }

    //LOBBY SCREEN

    public void OnJoined()
    {
        SetScreen(lobbyScreen);
        UpdateLobbyUI();
    }

    void UpdateLobbyUI ()
    {
        //display all players
        playerListText.text = "";

        foreach (PlayerConfig pc in PlayerConfigManager.instance.playerConfigs)
            playerListText.text += pc.Player.nickname + "\n";

        // set room info text
        roomInfoText.text = "<b>Room Name</b> \n" + "Change this";
    }

    public void OnStartGameButton ()
    {
        SceneManager.LoadScene(selectedStage);
    }

    public void OnLeaveLobbyButton ()
    {
        SetScreen(mainScreen);
    }
}
