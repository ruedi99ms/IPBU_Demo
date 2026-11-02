using System.Security.Cryptography;
using UnityEngine;

public class ParticleSystemFollower : MonoBehaviour
{

    public Transform player;
    public float heightOffset = -73.5f; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            transform.position = player.position - Vector3.up * heightOffset;
        }
    }
}
