using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameInformation : MonoBehaviour
{
    public TMP_Text coinsTxt, gemsTxt, scoreTxt, pointsTxt, highscoreTxt;
    public List<TMP_Text> colourText, giftTimers;
    [SerializeField] private List<Image> images, backgrounds;
    [SerializeField] private List<GameObject> hiddenObjects;
    public List<GameObject> forgrounds;

    public GameObject ballPrefab, playerPrefab, xObj, xPanel, settingsBtn, backBtn, particles;
    public BallScriptable ball;
    public ShoeScriptable shoe;
    public DatabaseManager databaseManager;
    public Canvas canvas;
    public Camera cam;
    public GameObject mainMenu, gameOver, wealth, settings, walls, reloadInternet, statusBar, main;
    public AudioClip[] bounceClips;

    public static int coins, gems, highscore, points, gameCoins, gameGems, games, round, errors, shoeSide;
    public static bool inPlay = false, giftReady = false;
    public static float camHeight, camWidth, h, w;

    public Banner bannerAd;
    public RewardedAds rewardedAd;
    public InterstitialAds interstitialAd;

    public AndroidNotification notification;
    public int notificationID;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        gems = databaseManager.playerData.playerAttributesData.gems;
        coins = databaseManager.playerData.playerAttributesData.coins;
        highscore = databaseManager.playerData.playerAttributesData.highscore;
        camHeight = cam.GetComponent<Camera>().orthographicSize * 2;
        camWidth = cam.GetComponent<Camera>().orthographicSize;
        h = camHeight / canvas.pixelRect.height;
        w = camWidth / canvas.pixelRect.width;

        List<ShoeScriptable> shoes = Resources.LoadAll<ShoeScriptable>("Scriptable Objects/Shoe/" + databaseManager.playerData.playerAttributesData.selectedShoe.Split(':')[0]).ToList();
        shoes.Sort((x, y) => x.id.CompareTo(y.id));
        shoe = shoes[int.Parse(databaseManager.playerData.playerAttributesData.selectedShoe.Split(':')[1])];
        playerPrefab.GetComponent<MeshFilter>().sharedMesh = shoe.prefab.GetComponent<MeshFilter>().sharedMesh;

        List<BallScriptable> balls = Resources.LoadAll<BallScriptable>("Scriptable Objects/Balls/" + databaseManager.playerData.playerAttributesData.selectedBall.Split(':')[0]).ToList();
        balls.Sort((x, y) => x.id.CompareTo(y.id));
        ball = balls[int.Parse(databaseManager.playerData.playerAttributesData.selectedBall.Split(":")[1])];
        ballPrefab.GetComponent<MeshFilter>().sharedMesh = ball.prefab.GetComponent<MeshFilter>().sharedMesh;
        cam.backgroundColor = ball.background;
        coinsTxt.text = coins.ToString();
        gemsTxt.text = gems.ToString();
        colourText.ForEach(text => text.color = new Color(ball.background.r + 0.2f, ball.background.g + 0.2f, ball.background.b + 0.2f, ball.background.a));
        images.ForEach(image => image.color = new Color(ball.background.r + 0.3f, ball.background.g + 0.3f, ball.background.b + 0.3f, ball.background.a));
        backgrounds.ForEach(image => image.color = ball.background);
        forgrounds.ForEach(image => { image.GetComponent<Image>().color = ball.foreground; });
        bounceClips = ball.bounceClips;

        highscoreTxt.text = highscore.ToString();
        shoeSide = databaseManager.playerPrefs.shoeSide;
        playerPrefab.transform.localScale = new Vector3(playerPrefab.transform.localScale.x, playerPrefab.transform.localScale.y, shoeSide * Mathf.Abs(playerPrefab.transform.localScale.z));
        startTimers();

        WallScript.generateWalls(0);

        if (databaseManager.playerPrefs.offline)
            hiddenObjects.ForEach(obj => { obj.SetActive(false); });

        if (!databaseManager.playerData.playerAttributesData.removeAds & !databaseManager.playerPrefs.offline)
            bannerAd.LoadAd();

        if (!databaseManager.playerPrefs.offline)
        {
            reloadInternet.SetActive(false);
        }
        else
        {
            reloadInternet.SetActive(true);
        }

        int barHeight = databaseManager.GetStatusBarHeight();
        statusBar.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1 - barHeight / canvas.pixelRect.height);
        statusBar.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        statusBar.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        statusBar.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        main.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        main.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1 - barHeight / canvas.pixelRect.height);
        main.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        main.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        settingsBtn.SetActive(true);
        backBtn.SetActive(false);

        AndroidNotificationCenter.CancelAllNotifications();

        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Prizes",
            Importance = Importance.High,
            Description = "Prize notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        notification = new AndroidNotification();
        notification.Title = "Open Gift";
        notification.Text = "Your gift is ready!";
        notification.SmallIcon = "logo_small";
        notification.LargeIcon = "gift";
        notification.ShowTimestamp = true;
        notification.ShouldAutoCancel = true;

        DateTime date = Convert.ToDateTime(databaseManager.playerData.playerAttributesData.gift.Replace(";", ":"));
        notification.FireTime = ((date - DateTime.UtcNow).TotalMilliseconds < 0) ? DateTime.UtcNow.AddMinutes(30) : date;

        notificationID = AndroidNotificationCenter.SendNotification(notification, "channel_id");

        var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationID);


        if (notificationStatus == NotificationStatus.Scheduled)
        {
            // Replace the scheduled notification with a new notification.
            AndroidNotificationCenter.UpdateScheduledNotification(notificationID, notification, "channel_id");
        }
        else if (notificationStatus == NotificationStatus.Delivered)
        {
            // Remove the previously shown notification from the status bar.
            AndroidNotificationCenter.CancelNotification(notificationID);
        }
        else if (notificationStatus == NotificationStatus.Unknown)
        {
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }

    public void startTimers()
    {
        StopAllCoroutines();
        DateTime date = new DateTime();
        date = Convert.ToDateTime(databaseManager.playerData.playerAttributesData.gift.Replace(";", ":")).AddMinutes(30);
        if ((date - DateTime.UtcNow).TotalMilliseconds < 0)
        {
            giftReady = true;
            giftTimers.ForEach(timer => timer.text = "Ready!");
            StartCoroutine(readyAnimation());
        }
        else
        {
            giftReady = false;
            StartCoroutine(updateTime(date, 30));
        }
    }

    public IEnumerator readyAnimation()
    {
        giftTimers.ForEach(async timer =>
        {
            LeanTween.scale(timer.gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.2f).setEaseInCirc();
            await Task.Delay(200);
            LeanTween.scale(timer.gameObject, Vector3.one, 0.3f).setEaseOutBack();
        });
        yield return new WaitForSeconds(0.55f);
        StartCoroutine(readyAnimation());
    }

    public IEnumerator updateTime(DateTime openedTime, int timeDisparity)
    {
        string formatedDate;
        formatedDate = (openedTime - DateTime.UtcNow).ToString("mm':'ss''");
        formatedDate = ((openedTime - DateTime.UtcNow).TotalSeconds <= 60) ? string.Format("{0}s", formatedDate.Split(":")[1]) : string.Format("{0}m {1}s", formatedDate.Split(":")[0], formatedDate.Split(":")[1]);
        formatedDate = ((openedTime - DateTime.UtcNow).TotalMinutes <= 0) ? "Ready!" : formatedDate;
        giftTimers.ForEach(timer => timer.text = formatedDate);

        if (formatedDate == "Ready!")
        {
            giftReady = true;
            startTimers();
            yield break;
        }

        yield return new WaitForSecondsRealtime(1);

        StartCoroutine(updateTime(openedTime, timeDisparity));
    }

    public void loginScreen()
    {
        SceneManager.LoadScene("Login");
    }
}
