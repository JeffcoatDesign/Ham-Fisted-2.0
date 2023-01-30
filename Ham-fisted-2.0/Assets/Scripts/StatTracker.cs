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
    }
    #endregion

    public Vector2[] kos { get; private set; }
    public List<int> sds;
    
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

    public void AddKO (int hitter, int fell)
    {
        List<Vector2> tempList;
        if (kos != null)
            tempList = new List<Vector2>(kos);
        else 
            tempList = new();
        tempList.Add(new(hitter, fell));
        kos = tempList.ToArray();
    }

    public void AddSD (int player)
    {
        sds.Add(player);
    }
}
