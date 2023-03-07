using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punchable : MonoBehaviour
{
    public int lastHitBy = -1;
    public bool CanRespawn = false;

    [SerializeField] private Rigidbody rig;
    [SerializeField] private Transform respawnPoint;

    public virtual void GetHit(Vector3 forward, float force, int attackerID)
    {
        lastHitBy = attackerID;
        Vector3 launchDir = (forward).normalized;
        rig.AddForce(launchDir * force, ForceMode.Impulse);
    }

    public virtual void Respawn()
    {
        transform.position = respawnPoint.position;
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (GameManager.instance != null)
        {
            if (rig.position.y < GameManager.instance.fallY)
            {
                if (CanRespawn)
                    Respawn();
            }
        }
    }
}
