using System;
using KvRaeScripts;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PhonePosition : MonoBehaviour
{
    private const float positionOffsetThreshold = 0.5f;
    private const float accelerationThreshold = 9.8f;
    private const float rotationThreshold = 500.0f; 
    private const float shockThreshold = 10f; 
    private float latitude;
    private float longitude;

    private Quaternion _rotation;
    private Vector3 _position;
    private Vector3 _acceleration;
    private Vector3 currentPosition;
    private bool isFalling;
    private bool isRotating;

    private float rotationSpeed; // Track the rotation speed
    private float lastDetectionTime;

    private MobileNotificationManager _mobileNotificationManager;
    private MailingService _mailingService;

    public Image activityImage;
    public TextMeshProUGUI activityTracker;
    public Sprite fallSprite; // Add a reference to your fall image
    public Sprite shockSprite; // Add a reference to your shock image
    public Button falseAlarmButton;
    public TextMeshProUGUI falseAlarmText;

    private string recipient;

    private void Awake()
    {
        // Check if location services are enabled
        GpsLocationInit();
        if (PlayerPrefs.HasKey("Email"))
        {
            recipient = PlayerPrefsManager.GetEmail();
        }
        else
        {
            recipient = "exemple@mail.com";
        }
        _mobileNotificationManager = new MobileNotificationManager();
        _mailingService = new MailingService();
        
        Application.runInBackground = true;
        
        falseAlarmButton.gameObject.SetActive(false);
        falseAlarmText.gameObject.SetActive(false);
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
        _acceleration = Input.gyro.userAcceleration;
        
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        // Check for fall
        CheckFall();
        // Check for fast rotation
        CheckRotationSpeed();
        // Check for shock
        CheckShock();
        
    }



    private void FixedUpdate()
    {
        currentPosition = _position;
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
                activityImage.sprite = fallSprite; // Change the image on fast rotation
                if (PlayerPrefsManager.GetMailServiceActivation())
                {
                    
                    falseAlarmButton.gameObject.SetActive(true);
                    falseAlarmText.gameObject.SetActive(true);
                }

                if (PlayerPrefsManager.alarmSent == false)
                {
                    PlayerPrefsManager.alarmSent = true;
                    SendNotification("Rotation detected", "Fast rotation detected!");
                    SendEmail("Rotation detected", "Fast rotation detected!");
                }
                
                // Add any other action you want to perform on fast rotation
            }
        }
        else
        {
            isRotating = false;
        }
    }

    private void CheckShock()
    {
        if (Input.acceleration.sqrMagnitude > shockThreshold * shockThreshold )//&&  Time.time - lastDetectionTime > 1f) // Add a cooldown for shock detection
        {
            // Shock detected, perform your actions here
            activityTracker.text = "Shock detected!";
            activityImage.sprite = shockSprite; // Change the image on fast rotation
            if (PlayerPrefsManager.GetMailServiceActivation())
            {
                falseAlarmButton.gameObject.SetActive(true);
                falseAlarmText.gameObject.SetActive(true);
            }
            
            vibration();
            if (PlayerPrefsManager.alarmSent == false)
            {
                PlayerPrefsManager.alarmSent = true;
                SendNotification("Shock detected", "Fast Shock detected!");
                SendEmail("Shock detected", "Fast Shock detected!");
            }
            lastDetectionTime = Time.time;
        }
    }
    
    private void CheckFall()
    {
        if (_acceleration.magnitude > accelerationThreshold)
        {
            isFalling = true;
            activityTracker.text = "Fall detected!";
            activityImage.sprite = fallSprite; // Change the image on fall detection
            if (PlayerPrefsManager.GetMailServiceActivation())
            {
                
                falseAlarmButton.gameObject.SetActive(true);
                falseAlarmText.gameObject.SetActive(true);
            }
            
            vibration();
            if (PlayerPrefsManager.alarmSent == false)
            {
                PlayerPrefsManager.alarmSent = true;
                SendEmail("I need your help", "I fell down, please help me!");
                SendNotification("Fall detected", "Are you Ok? an fall was detected!");
            }
            
        }
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
    
    private void vibration()
    {
        if (SystemInfo.supportsVibration)
        {
            Handheld.Vibrate();
        }
    }
    
    private void SendEmail(string _subject, string _body)
    {
        var url = "";
        if (latitude != 0 && longitude != 0)
        { 
            url = "https://www.google.com/maps/search/?api=1&query=" + latitude + "," + longitude;
        }
        
        
        if ( PlayerPrefsManager.GetMailServiceActivation())
        {
            _mailingService.SendEmail(recipient: recipient, subject: _subject, body: _body, cords: url);
        }
        
    }
    
    private void SendNotification(string _title, string _description)
    {
        if (PlayerPrefsManager.GetNotificationServiceActivation())
        {
            _mobileNotificationManager.SendNotification(_title, _description);
        }
    }

    private void GpsLocationInit()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location services are not enabled.");
            return;
        }


        // Start location service updates
        Input.location.Start();

        // Wait until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            maxWait--;
            Debug.Log("Waiting for location services to initialize...");
            System.Threading.Thread.Sleep(1000);
        }

        // Check if the location service has initialized
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location.");
            return;
        }
    }

    private void OnDestroy()
    {
        Input.location.Stop();
    }
}
