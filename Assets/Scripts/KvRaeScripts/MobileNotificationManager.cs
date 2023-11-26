using System;
using Unity.Notifications.Android;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

namespace KvRaeScripts
{
    public class MobileNotificationManager : MonoBehaviour
    {
        public void RequestAuthorisation()
        {
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATION"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATION");
            }
        
        }


    
        public void SendNotification(string title, string description)
        {
            try {
                var channel = new AndroidNotificationChannel()
                {
                    Id = "channel_id",
                    Name = "Default Channel",
                    Importance = Importance.High,
                    Description = "Generic notifications"
                };
                AndroidNotificationCenter.RegisterNotificationChannel(channel);
                var notification = new AndroidNotification
                {
                    Title = title,
                    Text = description,
                    SmallIcon = "default",
                    LargeIcon = "default",
                    FireTime = DateTime.Now.AddSeconds(15)
                };
                AndroidNotificationCenter.SendNotification(notification, channel.Id);
                Debug.Log("Notification sent successfully !");
        
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        
        
        }

        private void Start()
        {
            RequestAuthorisation();
            SendNotification("welcome to take care", "this is a welcome notification");
        
        }

    }
}
