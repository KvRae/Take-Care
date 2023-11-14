using KvRaeScripts;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PhonePosition : MonoBehaviour
{
    private const float positionOffsetThreshold = 0.5f;
    private const float accelerationThreshold = 9.8f;
    private const float rotationThreshold = 500.0f; // Adjust as needed

    private Quaternion _rotation;
    private Vector3 _position;
    private Vector3 _acceleration;
    private Vector3 currentPosition;
    private bool isFalling;
    private bool isRotating;

    private float rotationSpeed; // Track the rotation speed

    private MobileNotificationManager _mobileNotificationManager;
    private MailingService _mailingService;

    public Image activityImage;
    public TextMeshProUGUI activityTracker;

    private void Awake()
    {
        _mobileNotificationManager = new MobileNotificationManager();
        _mailingService = new MailingService();
        Application.runInBackground = true;
    }

    private void Start()
    {
        if (!SystemInfo.supportsGyroscope)
        {
            Debug.LogError("Gyroscope is not supported on this device.");
            return;
        }

        Input.gyro.enabled = true;
        _position = transform.position;
        activityTracker.text = "You are all good";
    }

    private void Update()
    {
        _position = transform.position;

        if (!SystemInfo.supportsGyroscope) return;

        _rotation = GyroToUnity(Input.gyro.attitude);
        var _gravity = Input.gyro.gravity;
        _acceleration = Input.gyro.userAcceleration;

        if (_acceleration.magnitude > accelerationThreshold)
        {
            isFalling = true;
            activityTracker.text = "Fall detected!";
            Debug.Log("Fall detected!");
            _mobileNotificationManager.SendNotification("Fall detected", "A fall was detected!");
            _mailingService.SendEmail("karamelmannai@gmail.com", "Fall Detected", "A fall was detected!");
        }
        else
        {
            // Check for fast rotation
            CheckRotationSpeed();
        }
    }

    private void CheckRotationSpeed()
    {
        var deltaRotation = Quaternion.Angle(transform.rotation, _rotation);

        if (deltaRotation > rotationThreshold)
        {
            if (!isRotating)
            {
                isRotating = true;
                activityTracker.text = "Fast rotation detected!";
                Debug.Log("Fast rotation detected!");
                _mobileNotificationManager.SendNotification("Rotation detected", "Fast rotation detected!");
                // Add any other action you want to perform on fast rotation
            }
        }
        else
        {
            isRotating = false;
        }
    }

    private void FixedUpdate()
    {
        currentPosition = _position;

        var offset = Vector3.Distance(_position, currentPosition);

        if (offset > positionOffsetThreshold && !isFalling)
        {
            isFalling = true;
            _mobileNotificationManager.SendNotification("Fall detected", "A fall was detected!");
            activityTracker.text = "Fall detected!";
            _mailingService.SendEmail(recipient: "karamelmannai@gmail.com", subject: "Fall detected", body: "A fall was detected!");
        }
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
