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

    public Vector2[] kos;
    public List<int> sds;
    
    public float time;
    [HideInInspector] public string gamemode;
    [HideInInspector] public string stage;
    [HideInInspector] public bool isTimeBased = false;
    private void Start()
    {
        PlayerConfigManager.instance.playerLeft.AddListener(RemovePlayer);
    }

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

    public void RemovePlayer (int player)
    {
        if (kos != null)
        {
            List<Vector2> newKOsList = new();
            foreach (var ko in kos)
            {
                if (ko.x != player && ko.y != player)
                    newKOsList.Add(ko);
            }
            newKOsList.ForEach(v => v = CheckIfAfter(v, player));
            kos = newKOsList.ToArray();
        }
        if (sds != null)
        {
            List<int> newSDsList = new();
            foreach (int sd in sds)
            {
                if (sd != player)
                    newSDsList.Add(sd);
            }
            newSDsList.ForEach(i => i = CheckIfAfter(i, player));
            sds = newSDsList;
        }
    }

    private Vector2 CheckIfAfter (Vector2 vect, int player)
    {
        Vector2 newVect = vect;
        if (player < vect.x)
            newVect.x -= 1;
        if (player < vect.y)
            newVect.y -= 1;
        return newVect;
    }
    private int CheckIfAfter(int i, int player)
    {
        int newI = i;
        if (player < i)
            newI -= 1;
        return newI;
    }
}
