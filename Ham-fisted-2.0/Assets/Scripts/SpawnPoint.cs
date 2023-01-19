using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool isCollidingWithPlayer = false;

    private void Start()
    {
        List<SpawnPoint> spawnPoints = new List<SpawnPoint>(GameManager.instance.spawnPoints);
        spawnPoints.Add(this);
        GameManager.instance.spawnPoints = spawnPoints.ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        isCollidingWithPlayer = other.CompareTag("Player");
    }

    private void OnTriggerExit(Collider other)
    {
        isCollidingWithPlayer = false;
    }
}
