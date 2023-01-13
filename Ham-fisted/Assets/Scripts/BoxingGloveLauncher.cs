using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BoxingGloveLauncher : MonoBehaviour
{
    public int id;
    public bool canHit = false;
    public float force;
    public string impactName;
    private bool hittingPlayer = false;
    private void OnTriggerStay(Collider other)
    {
        if (!canHit)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().photonView.RPC("GetHit", RpcTarget.All, transform.position, force, id);
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
            GameObject hitObj = PhotonNetwork.Instantiate(impactName, transform.position + launchDir, rot, 0);
            hittingPlayer = false;
            StartCoroutine(DestroyHit(hitObj));
        }
    }

    private IEnumerator DestroyHit(GameObject obj)
    {
        // Debug.Log("Destroying hit");

        yield return new WaitForSeconds(1f);
        PhotonNetwork.Destroy(obj.GetPhotonView());
    }
}
