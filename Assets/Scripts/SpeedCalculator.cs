using UnityEngine;
using TMPro;

public class SpeedCalculator : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    private float speedKMPH;

    private Vector3 lastPosition;
    private float lastTime;
    private float movementThreshold = 0.1f;
    public float speedLimit = 100.0f;
    private bool exceededSpeedLimitNotified = false;

    [SerializeField]
    public GameObject audioSourceGameObject; // Reference to the GameObject with the AudioSource component

    private void Start()
    {
        lastPosition = Input.acceleration;
        lastTime = Time.time;
    }

    private void Update()
    {
        Vector3 currentAcceleration = Input.acceleration;
        float deltaTime = Time.time - lastTime;

        float distance = Vector3.Distance(currentAcceleration, lastPosition);
        float speedMPS = distance / deltaTime;

        // Convert speed to kilometers per hour (km/h).
        speedKMPH = speedMPS ; // 1 m/s = 3600 km/h

        lastPosition = currentAcceleration;
        lastTime = Time.time;

        if (speedKMPH < movementThreshold)
        {
            speedText.text = "Speed : ";
            exceededSpeedLimitNotified = false; // Reset the flag
        }
        else
        {
            speedText.text = "Speed : " + speedKMPH.ToString("F0");

            if (speedKMPH > speedLimit && !exceededSpeedLimitNotified)
            {
                NotifySpeedLimitExceeded();
            }
        }
    }

    private void NotifySpeedLimitExceeded()
    {
        Debug.Log("Speed limit exceeded. Slow down!");

        // Play the audio clip on the GameObject with the AudioSource component
        if (audioSourceGameObject != null)
        {
            AudioSource audioSource = audioSourceGameObject.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }

        exceededSpeedLimitNotified = true;
    }
}
