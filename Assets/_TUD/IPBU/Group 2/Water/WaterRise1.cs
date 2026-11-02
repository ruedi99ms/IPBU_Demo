using UnityEngine;

public class WaterRise : MonoBehaviour
{
    public float riseSpeed = 0.5f;
    public float maxHeight = 10f;
    private bool isRising = false;

    public void StartRising()
    {
        isRising = true;
    }

    private void Update()
    {
        if (isRising && transform.position.y < maxHeight)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }
    }
}
