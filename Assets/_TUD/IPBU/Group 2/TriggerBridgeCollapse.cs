using UnityEngine;

public class TriggerBridgeCollapse : MonoBehaviour
{
    private SimulationController simulationController;

    private void Start()
    {
        simulationController = FindObjectOfType<SimulationController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collide");

        if (other.CompareTag("Player"))
        {
            simulationController.TriggerBridgeCollapse();
        }
    }
}
