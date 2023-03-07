using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGloveLauncher : MonoBehaviour
{
    public int id;
    public float force;
    public float charge = 0;
    public bool wasBlockChecked = false;
    [SerializeField] private Collider boxingGloveCollider;
    [SerializeField] private GameObject impact;
    [SerializeField] private GameObject shieldEffect;
    [SerializeField] private float chargeMult = 1f;
    [SerializeField] private BoxingGloveController bGC;
    [SerializeField] private PlayerController pc;
    private bool hittingPlayer = false;

    public void ToggleCollider (bool enabled)
    {
        boxingGloveCollider.enabled = enabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Punchable") && !bGC.wasBlocked)
        {
            other.gameObject.GetComponent<Punchable>().GetHit(transform.forward, force + (charge * chargeMult), id);
        }
        if (other.gameObject.CompareTag("Player") && wasBlockChecked && !bGC.wasBlocked)
        {
            other.gameObject.GetComponent<PlayerController>().GetHit(transform.forward, force + (charge * chargeMult), id);
            hittingPlayer = true;
        }
        if (other.gameObject.CompareTag("Shield"))
        {
            if (other.gameObject.transform.parent != gameObject.transform.parent.transform.parent)
            {
                Debug.Log("Being blocked");
                bGC.wasBlocked = true;
                ToggleCollider(false);

                Vector3 launchDir = other.transform.position - transform.position;
                Quaternion rot = Quaternion.LookRotation(launchDir, Vector3.up);
                GameObject hitObj = Instantiate(shieldEffect, transform.position + launchDir, rot);
                StartCoroutine(DestroyHit(hitObj));
                pc.StartStun();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && wasBlockChecked && hittingPlayer)
        {
            Vector3 launchDir = other.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(launchDir, Vector3.up);
            GameObject hitObj = Instantiate(impact, transform.position + launchDir, rot);
            hittingPlayer = false;
            StartCoroutine(DestroyHit(hitObj));
        }
        if (other.gameObject.CompareTag("Shield"))
        {
            Debug.Log("Being blocked");
            bGC.wasBlocked = true;
            ToggleCollider(false);
            pc.StartStun();
        }
    }

    private IEnumerator DestroyHit(GameObject obj)
    {
        // Debug.Log("Destroying hit");

        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }
}
