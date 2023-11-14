using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public Button emailServiceButton;
    public TextMeshProUGUI emailServiceText;
    public Button notificationServiceButton;
    public TextMeshProUGUI notificationServiceText;
    bool isEmailServiceActive;
    bool isNotificationServiceActive;

    private void Awake()
    {
        // Check if MailService is set in PlayerPrefs
        if (!PlayerPrefs.HasKey("MailService"))
        {
            // Set initial value if not set
            PlayerPrefsManager.SetMailServiceActivation(true);
        }

        // Update UI text for MailService
        isEmailServiceActive = PlayerPrefsManager.GetMailServiceActivation();
        if (isEmailServiceActive)
        {
            emailServiceText.text = "Service enabled";
        }
        else
        {
            emailServiceText.text = "Service disabled";
        }

        // Check if NotificationService is set in PlayerPrefs
        if (!PlayerPrefs.HasKey("NotificationService"))
        {
            // Set initial value if not set
            PlayerPrefsManager.SetNotificationServiceActivation(true);
        }

        isNotificationServiceActive = PlayerPrefsManager.GetNotificationServiceActivation();
        if (isNotificationServiceActive)
        {
            notificationServiceText.text = "Service enabled";
        }
        else
        {
            notificationServiceText.text = "Service disabled";
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (emailServiceButton == null)
        {
            Debug.LogError("Email service button not set");
        }
        else
        {
            emailServiceButton.onClick.AddListener(onChangeEmailService);
        }

        if (notificationServiceButton == null)
        {
            Debug.LogError("Notification service button not set");
        }
        else
        {
            notificationServiceButton.onClick.AddListener(onChangeNotificationService);
        }
    }

    void Update()
    {
        // Change the text of the button based on the value of the MailService
        if (emailServiceText != null && isEmailServiceActive != PlayerPrefsManager.GetMailServiceActivation())
        {
            isEmailServiceActive = !isEmailServiceActive;
            emailServiceText.text = isEmailServiceActive ? "Service enabled" : "Service disabled";
        }

        // Change the text of the button based on the value of the NotificationService
        if (notificationServiceText != null && isNotificationServiceActive != PlayerPrefsManager.GetNotificationServiceActivation())
        {
            isNotificationServiceActive = !isNotificationServiceActive;
            notificationServiceText.text = isNotificationServiceActive ? "Service enabled" : "Service disabled";
        }
    }

    public void onChangeEmailService()
    {
        Debug.Log("Email service button clicked");
        isEmailServiceActive = !isEmailServiceActive;
        PlayerPrefsManager.SetMailServiceActivation(isEmailServiceActive);
    }

    public void onChangeNotificationService()
    {
        Debug.Log("Notification service button clicked");
        isNotificationServiceActive = !isNotificationServiceActive;
        PlayerPrefsManager.SetNotificationServiceActivation(isNotificationServiceActive);
    }
}
