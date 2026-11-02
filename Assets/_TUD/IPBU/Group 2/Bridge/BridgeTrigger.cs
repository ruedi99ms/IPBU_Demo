using UnityEngine;

public class BridgeCollapse : MonoBehaviour
{
    public GameObject bridgeParent; // Reference to the "Bridge" parent object
    public string triggeringTag = "Player"; // Tag for triggering object (e.g., "Player")
    public float collapseForce = 10f; // Force applied to bridge pieces
    public float upwardForce = 2f; // Upward force for explosion effect

    private Rigidbody[] bridgePieces; // Array to store rigidbodies of bridge pieces
    private ParticleSystem[] particleSystems; // Array to store particle systems

    void Start()
    {
        // Get all rigidbody components from bridge pieces
        bridgePieces = bridgeParent.GetComponentsInChildren<Rigidbody>();

        // Ensure all pieces start static
        foreach (Rigidbody piece in bridgePieces)
        {
            piece.isKinematic = true;
        }

        // Get all particle systems from the bridge
        particleSystems = bridgeParent.GetComponentsInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the triggering object matches the specified tag
        if (other.CompareTag(triggeringTag))
        {
            CollapseBridge();
            PlayParticles();
        }
    }

    void CollapseBridge()
    {
        // Apply force to each bridge piece to simulate collapse
        foreach (Rigidbody piece in bridgePieces)
        {
            piece.isKinematic = false; // Enable physics simulation
            Vector3 randomForce = Vector3.up * upwardForce + Random.insideUnitSphere * collapseForce;
            piece.AddForce(randomForce, ForceMode.Impulse);
        }
    }

    void PlayParticles()
    {
        // Trigger all particle systems
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }
    }
}
