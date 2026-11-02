using System.Collections;
using UnityEngine;

public class BridgeCollapseTrigger : MonoBehaviour
{

    public GameObject[] bridgeParts;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject part in bridgeParts)
            {
                Rigidbody rb = part.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
        }
    }



}
