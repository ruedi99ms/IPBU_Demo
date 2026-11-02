using UnityEngine;

public class SafeDestinationTrigger : MonoBehaviour
{
    private SimulationController simulationController;

    private void Start()
    {
        simulationController = FindObjectOfType<SimulationController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            simulationController.OnSafeDestinationReached();
        }
    }
}
