using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    //Changes the scene after the client connects tp the master server
    private void Load()
    {
        SceneManager.LoadScene("Menu");
    }

    private void Start()
    {
        Invoke("Load", 1f);
    }
}
