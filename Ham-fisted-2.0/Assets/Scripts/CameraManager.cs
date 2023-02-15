using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public Camera p1Cam;
    public Camera p2Cam;
    public Camera p3Cam;
    public Camera p4Cam;
    public PlayerGameUI[] playerGameUIs;

    public void Start()
    {
        instance = this;
    }
    public void Initiate(int numLocPlayers)
    {
        if (numLocPlayers == 2)
        {
            p1Cam.rect = new Rect(0, 0, 0.5f, 1);
            p2Cam.rect = new Rect(0.5f, 0, 0.5f, 1);
            p2Cam.gameObject.SetActive(true);
            playerGameUIs[1].gameObject.SetActive(true);
        }
        else if (numLocPlayers == 3)
        {
            p1Cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            p2Cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            p2Cam.gameObject.SetActive(true);
            p3Cam.gameObject.SetActive(true);
            p4Cam.gameObject.SetActive(true);
            playerGameUIs[0].gameObject.SetActive(true);
            playerGameUIs[1].gameObject.SetActive(true);
            playerGameUIs[2].gameObject.SetActive(true);
        }
        else if (numLocPlayers == 4)
        {
            p1Cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            p2Cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            p2Cam.gameObject.SetActive(true);
            p3Cam.gameObject.SetActive(true);
            p4Cam.gameObject.SetActive(true);
            playerGameUIs[0].gameObject.SetActive(true);
            playerGameUIs[1].gameObject.SetActive(true);
            playerGameUIs[2].gameObject.SetActive(true);
            playerGameUIs[3].gameObject.SetActive(true);
        }
        playerGameUIs[0].gameObject.SetActive(true);
    }
}
