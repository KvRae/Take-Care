using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{
    // Keys for the PlayerPrefs
    private const string MailServiceKey = "MailService";
    private const string NotificationServiceKey = "NotificationService";
    private const string EmailKey = "Email";
    public static bool alarmSent = false;

    // Setters
    public static void SetMailServiceActivation(bool value)
    {
        PlayerPrefs.SetInt(MailServiceKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public static void SetNotificationServiceActivation(bool value)
    {
        PlayerPrefs.SetInt(NotificationServiceKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    // Set Email in PlayerPrefs
    public static void SetEmail(string email)
    {
        PlayerPrefs.SetString(EmailKey, email);
        PlayerPrefs.Save();
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
    
    // Get Email from PlayerPrefs
    public static string GetEmail()
    {
        
        return PlayerPrefs.GetString(EmailKey);
        
    }
}