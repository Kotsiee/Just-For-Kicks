using TMPro;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class JFKNotification : MonoBehaviour
{
    private static AndroidJavaObject _wifiManager;
    public TMP_Text test;

    void Start()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Prizes",
            Importance = Importance.High,
            Description = "Prize notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = "Open Gift";
        notification.Text = "Your gift is ready!";
        notification.SmallIcon = "prize_small";
        notification.LargeIcon = "logo";

        notification.FireTime = System.DateTime.Now.AddSeconds(20);

        var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");

        var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(id);


        if (notificationStatus == NotificationStatus.Scheduled)
        {
            // Replace the scheduled notification with a new notification.
            AndroidNotificationCenter.UpdateScheduledNotification(id, notification, "channel_id");
        }
        else if (notificationStatus == NotificationStatus.Delivered)
        {
            // Remove the previously shown notification from the status bar.
            AndroidNotificationCenter.CancelNotification(id);
        }
        else if (notificationStatus == NotificationStatus.Unknown)
        {
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }
}
