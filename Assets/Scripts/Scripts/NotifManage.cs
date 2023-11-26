using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using Unity.Notifications.Android;

public class NotifManage : MonoBehaviour
{

    private void Start()
    {
        RegisterNotificationChannel();
        ScheduleRepeatingNotifications();
    }

    public void RegisterNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void ScheduleRepeatingNotifications()
    {
        // Get the current time
        System.DateTime currentTime = System.DateTime.Now;

        // Set the start time at 8:00 AM
        System.DateTime startTime = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 8, 0, 0);

        // Set the end time at midnight
        System.DateTime endTime = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0).AddDays(1);

        while (startTime <= endTime)
        {
            var notification = new AndroidNotification();
            notification.Title = "Water Reminder";
            notification.Text = "Don't forget to drink water!";
            notification.FireTime = startTime;
            notification.SmallIcon = "small";
            notification.LargeIcon = "big";
            notification.ShowTimestamp = true;

            AndroidNotificationCenter.SendNotification(notification, "channel_id");

            // Schedule the next notification in 2 hours
            startTime = startTime.AddHours(2);
        }
    }

}
