using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public Button emailServiceButton;
    public TextMeshProUGUI emailServiceText;
    public Button notificationServiceButton;
    public TextMeshProUGUI notificationServiceText;
    
    // email modal
    public TMP_InputField emailInputField;
    public GameObject emailModal;
    public TextMeshProUGUI emailErrorModalText;
    
    public TextMeshProUGUI fallText;
    public Image fallImage;
    
    public Sprite notFallenSprite;
    
    bool isEmailServiceActive;
    bool isNotificationServiceActive;
    
    public Button falseAlarmButton;
    public TextMeshProUGUI falseAlarmText;
    
    private MailingService _mailingService;
    private string recipient;
    

    private void Awake()
    {

        if (PlayerPrefs.HasKey("Email"))
        {
            emailModal.SetActive(false);
            emailInputField.text = PlayerPrefsManager.GetEmail();
            recipient = PlayerPrefsManager.GetEmail();
        }
        else
        {
            emailModal.SetActive(true);
        }
        
        // Check if MailService is set in PlayerPrefs
        if (!PlayerPrefs.HasKey("MailService"))
        {
            // Set initial value if not set
            PlayerPrefsManager.SetMailServiceActivation(false);
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
            PlayerPrefsManager.SetNotificationServiceActivation(false);
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
        
        // false alarm button
        falseAlarmButton = GameObject.Find("FalseAlarmButton").GetComponent<Button>();
        falseAlarmText = GameObject.Find("FalseAlarmText").GetComponent<TextMeshProUGUI>();
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

    public void onChangeEmailService()
    {
        Debug.Log("Email service button clicked");
        isEmailServiceActive = !isEmailServiceActive;
        PlayerPrefsManager.SetMailServiceActivation(isEmailServiceActive);
        emailServiceText.text = isEmailServiceActive ? "Service enabled" : "Service disabled";
    }

    public void onChangeNotificationService()
    {
        Debug.Log("Notification service button clicked");
        isNotificationServiceActive = !isNotificationServiceActive;
        Debug.Log(isNotificationServiceActive.ToString());
        PlayerPrefsManager.SetNotificationServiceActivation(isNotificationServiceActive);
        notificationServiceText.text = isNotificationServiceActive ? "Service enabled" : "Service disabled";
    }

    public void onFalseAlarm()
    {
        _mailingService = new MailingService();
        _mailingService.SendEmail(recipient : recipient, subject:"False Alarm", body:"This is a false alarm. I am safe.", cords:"");
        falseAlarmButton.gameObject.SetActive(false);
        falseAlarmText.gameObject.SetActive(false);
        fallText.text = "You are all good";
        fallImage.sprite = notFallenSprite;
    }
    
    public void onEmailModalButton()
    {
        var emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        if (emailInputField.text != "" && emailRegex.IsMatch(emailInputField.text))
        {
            PlayerPrefsManager.SetEmail(emailInputField.text);
            emailModal.SetActive(false);
        }
        else
        {
            emailErrorModalText.gameObject.SetActive(true);
            emailErrorModalText.text = "Please enter a valid email address";
        }
    }
    
    public void onEmailModalToggleButton()
    {
        emailModal.SetActive(true);
    }
}
