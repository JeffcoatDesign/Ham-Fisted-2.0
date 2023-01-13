using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnthing : MonoBehaviour
{
    public GameObject hit;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetDelay());
    }

    IEnumerator SetDelay()
    {
        yield return new WaitForSeconds(2f);
        GameObject whatever = GameObject.Instantiate(hit);
        yield return new WaitForSeconds(0.5f);
        Destroy(whatever);
    }
}
