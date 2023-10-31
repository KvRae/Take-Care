using UnityEngine;
using TMPro;

public class PhonePosition : MonoBehaviour
{
    private const float positionOffsetThreshold = 0.5f; // offset between first snapshot and last snapshot
    private const float accelerationThreshold = 9.8f; // acceleration threshold to detect a fall
    private Quaternion _rotation;  // rotation of the device
    private Vector3 _position;  // position of the device
    private Vector3 _acceleration;  // acceleration of the device
    private bool isFalling;  // flag to indicate a fall

    public TextMeshProUGUI rotationText;    // text to display the rotation
    public TextMeshProUGUI accelerationText;  // text to display the acceleration

    // Start is called before the first frame update
    private void Awake()
    {
        Application.runInBackground = true;  // allow the app to run in the background
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (!SystemInfo.supportsGyroscope) return;
        Input.gyro.enabled = true;
        _position = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!SystemInfo.supportsGyroscope) return;
        _rotation = GyroToUnity(Input.gyro.attitude);
        var _gravity = Input.gyro.gravity;
        _acceleration = Input.gyro.userAcceleration;

        rotationText.text = "Gravity: " + _gravity;
        accelerationText.text = "Acceleration: " + _acceleration;

        // Check if acceleration exceeds the fall threshold
        if (!(_acceleration.magnitude > accelerationThreshold)) return;
        isFalling = true;
        Debug.Log("Fall detected!");
    }

    // FixedUpdate is called at a fixed interval
    private void FixedUpdate()
    {
        var currentPosition = transform.position; // Get the current position in FixedUpdate

        // Calculate the offset between the previous and current position
        var offset = Vector3.Distance(_position, currentPosition);

        if (offset > positionOffsetThreshold)
        {
            // Trigger your alert or perform any action here
            Debug.Log("Position offset exceeds threshold! Alert triggered.");
            _position = currentPosition;
        }

        // If a fall was detected earlier, you can take additional actions here
        if (!isFalling) return;
        Debug.Log("Fall detected! Alert triggered.");
        isFalling = false;
    }
    
    
    // Convert the gyro Quaternion to Unity's Quaternion.
    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
