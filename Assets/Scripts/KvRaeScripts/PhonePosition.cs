using KvRaeScripts;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class PhonePosition : MonoBehaviour
{
    private const float positionOffsetThreshold = 0.5f; // position offset threshold current value = 0.5f
    private const float accelerationThreshold = 9.8f; // acceleration threshold current value 9.8f
    
    
    private Quaternion _rotation;  // rotation of the device
    private Vector3 _position;  // position of the device
    private Vector3 _acceleration;  // acceleration of the device
    private Vector3 currentPosition;  // current position of the device
    
    private bool isFalling;  // flag to indicate a fall
    
    private MobileNotificationManager _mobileNotificationManager;  // mobile notification manager
    private MailingService _mailingService;  // mobile notification manager

    public Image activityImage;
    public TextMeshProUGUI activityTracker;
    //public TextMeshProUGUI rotationText;    // text to display the rotation
    //public TextMeshProUGUI accelerationText;  // text to display the acceleration

    // Start is called before the first frame update
    private void Awake()
    {
        _mobileNotificationManager = new MobileNotificationManager();
        _mailingService = new MailingService();
        Application.runInBackground = true;  // allow the app to run in the background
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (!SystemInfo.supportsGyroscope) return;
        Input.gyro.enabled = true;
        _position = transform.position;
        activityTracker.text = "You are all good "; 
        activityImage.sprite = Resources.Load<Sprite>("UI/Icons/ok");
    }

    // Update is called once per frame
    private void Update()
    {
        _position = transform.position;
        
        if (!SystemInfo.supportsGyroscope) return;
        _rotation = GyroToUnity(Input.gyro.attitude);
        var _gravity = Input.gyro.gravity;
        _acceleration = Input.gyro.userAcceleration;

        //rotationText.text = "Gravity: " + _gravity;
        //accelerationText.text = "Acceleration: " + _acceleration;

        // Check if acceleration exceeds the fall threshold
        if (!(_acceleration.magnitude > accelerationThreshold)) return;
        isFalling = true;
        activityTracker.text = "Fall detected !";
        Debug.Log("Fall detected!");
        _mobileNotificationManager.SendNotification("test notif", "FALL DETECTED !");
        //_mailingService.SendEmail("karamelmannai@gamil.com", "Fall Detected", "TestFall");
    }

    // FixedUpdate is called at a fixed interval
    private void FixedUpdate()
    {
        
        currentPosition = _position; // Get the current position in FixedUpdate
        
        Debug.Log("Current position: " + currentPosition);
        Debug.Log("floating position: " + _position);

        // Calculate the offset between the previous and current position
        var offset = Vector3.Distance(_position, currentPosition);

        if (offset > positionOffsetThreshold)
        {
            // Trigger your alert or perform any action here
            
            _mobileNotificationManager.SendNotification("Fall detected", "A fall was detected.");
            _mailingService.SendEmail(recipient : "karamelmannai@gmail.com", subject:"Fall detected", body:"A fall was detected.");
            _position = currentPosition;
        }

        // If a fall was detected earlier, an email will be triggered 
        if (!isFalling) return;
        //Debug.Log("Fall detected! Alert triggered.");
        //_mobileNotificationManager.SendNotification("Fall detected", "A fall was detected.");
        isFalling = false;
    }
    
    
    // Convert the gyro Quaternion to Unity's Quaternion.
    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
