using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{
    // Keys for the PlayerPrefs
    private const string MailServiceKey = "MailService";
    private const string NotificationServiceKey = "NotificationService";

    // Setters
    public static void SetMailServiceActivation(bool value)
    {
        PlayerPrefs.SetInt(MailServiceKey, value ? 1 : 0);
    }
    
    public static void SetNotificationServiceActivation(bool value)
    {
        PlayerPrefs.SetInt(NotificationServiceKey, value ? 1 : 0);
    }
    
    // Getters
    public static bool GetMailServiceActivation()
    {
        return PlayerPrefs.GetInt(MailServiceKey) == 1;
    }
    
    public static bool GetNotificationServiceActivation()
    {
        return PlayerPrefs.GetInt(NotificationServiceKey) == 1;
    }
}