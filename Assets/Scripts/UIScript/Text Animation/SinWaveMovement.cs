using UnityEngine;

public class SinWaveMovement : MonoBehaviour
{
    // Adjust these in the Inspector to control the wave
    public float amplitude = 1.0f; // Height of the wave
    public float frequency = 1.0f; // Speed of the wave
    public float horizontalSpeed = 2.0f; // Speed of forward movement

    private Vector3 startPosition;
    private float timeElapsed = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Increment time
        timeElapsed += Time.deltaTime * frequency;

        // Calculate vertical (Y axis) movement using the sine wave
        // The result of Mathf.Sin is between -1 and 1
        float yOffset = Mathf.Sin(timeElapsed) * amplitude;

        Debug.Log(yOffset);
        
        //float xOffset = Time.deltaTime * horizontalSpeed;
      
        transform.position +=transform.up * yOffset;
        //transform.position += transform.right * xOffset + transform.up * yOffset;
        
        // If you want to move relative to an initial direction and add sine on a perpendicular axis:
        // transform.position = startPosition + transform.forward * forwardProgress + transform.up * yOffset;
    }
}