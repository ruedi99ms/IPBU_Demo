using UnityEngine;

public class WaterRiseScript : MonoBehaviour
{
    public float riseSpeed = 0.1f;
    public float maxHeight = 43.0f;

    private Vector3 initialPosition;

    void Start()
    {
        
        initialPosition = transform.position;
        
    }

    void Update()
    {

        if (transform.position.y < initialPosition.y + maxHeight)
        {
            transform.position += new Vector3(0, riseSpeed * Time.deltaTime, 0);
        }


    }
}
