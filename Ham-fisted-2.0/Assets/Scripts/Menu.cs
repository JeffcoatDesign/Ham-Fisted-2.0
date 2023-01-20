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
    public Button startGameButton;

    [Header("Tutorial Screen")]
    public Button tutorialBackButton;

    [Header("Credits Screen")]
    public Button creditsBackButton;

    [Header("Cameras")]
    public GameObject menuVCam;
    public GameObject lobbyVCam;

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

        if (screen == lobbyScreen)
        {
            startGameButton.Select();
            menuVCam.SetActive(false);
            lobbyVCam.SetActive(true);
        }
        if (screen == mainScreen)
        {
            startButton.Select();
            menuVCam.SetActive(true);
            lobbyVCam.SetActive(false);
        }
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
    // MENU SCREEN

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

    public void OnFeedbackButton ()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSdUhQNas3QVP9-uNUv1NSDOjVwAxZjjk2LB2h7McDcg6bdNCw/viewform?usp=sf_link");
    }

    //LOBBY SCREEN

    public void OnStartGameButton ()
    {
        SceneManager.LoadScene(selectedStage);
    }

    public void OnLeaveLobbyButton ()
    {
        SetScreen(mainScreen);
    }
}
