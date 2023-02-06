using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGloveLauncher : MonoBehaviour
{
    public int id;
    public float force;
    public float charge = 0;
    [SerializeField] private Collider boxingGloveCollider;
    [SerializeField] private GameObject impact;
    [SerializeField] private float chargeMult = 1f;
    private bool hittingPlayer = false;

    public void ToggleCollider (bool enabled)
    {
        boxingGloveCollider.enabled = enabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().GetHit(transform.forward, force + (charge * chargeMult), id);
            hittingPlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!hittingPlayer)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 launchDir = other.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(launchDir, Vector3.up);
            GameObject hitObj = Instantiate(impact, transform.position + launchDir, rot);
            hittingPlayer = false;
            StartCoroutine(DestroyHit(hitObj));
        }
    }

    private IEnumerator DestroyHit(GameObject obj)
    {
        // Debug.Log("Destroying hit");

        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }
}
