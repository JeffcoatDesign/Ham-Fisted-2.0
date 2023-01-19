using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatTracker : MonoBehaviour
{
    #region instance
    public static StatTracker instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public int[] killedPlayers = new int[0];
    public int[] killedMe = new int[0];
    
    public float time;
    [HideInInspector] public string gamemode;
    [HideInInspector] public string stage;
    [HideInInspector] public bool isTimeBased = false;

    public void SetGamemode ()
    {
        gamemode = GameManager.instance.gamemode;
        stage = SceneManager.GetActiveScene().name;
        isTimeBased = GameManager.instance.isTimeBased;
    }

    public void AddKill (int index)
    {
        //Debug.Log("Knocked Out: Player " + index);
        List<int> tempList = new List<int>(killedPlayers);
        tempList.Add(index);
        killedPlayers = tempList.ToArray();
    }

    public void AddDeath (int index)
    {
        //Debug.Log("Knocked Down By: Player " + index);
        List<int> tempList = new List<int>(killedMe);
        tempList.Add(index);
        killedMe = tempList.ToArray();
    }
}
