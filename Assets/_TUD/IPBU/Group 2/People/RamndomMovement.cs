using UnityEngine;
using UnityEngine.AI;

public class RandomMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float wanderRadius = 20f;
    public float wanderTimer = 5f;

    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newDestination = GetRandomPoint(transform.position, wanderRadius);
            agent.SetDestination(newDestination);
            timer = 0;
        }

        // Trigger walk/run animation if needed
        if (agent.velocity.magnitude > 0.1f)
        {
            // Example of triggering "Run" animation
            GetComponent<Animator>().SetBool("isRunning", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("isRunning", false);
        }
    }

    Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, 1);
        return hit.position;
    }
}
