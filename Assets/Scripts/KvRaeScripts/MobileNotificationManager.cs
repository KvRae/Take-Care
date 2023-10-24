using System;
using Unity.Notifications.Android;
using UnityEngine;

public class MobileNotificationManager : MonoBehaviour
{
    
    private void Start()
    {
        // Create channel to send notifications messages
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        
        // Register the channel
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        
        // Send notification to user
        var notification = new AndroidNotification
        {
            Title = "Your Title",
            Text = "Your Text",
            FireTime = DateTime.Now.AddSeconds(10)
        };

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
}
