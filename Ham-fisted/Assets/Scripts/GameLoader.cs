using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameLoader : MonoBehaviourPunCallbacks
{
    //Changes the scene after the client connects tp the master server
    private void Load()
    {
        SceneManager.LoadScene("Menu");
    }
    public override void OnConnectedToMaster()
    {
        Load();
    }
}
