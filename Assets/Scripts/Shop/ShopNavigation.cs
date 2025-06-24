using GooglePlayGames;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopNavigation : MonoBehaviour
{
    [SerializeField] private GameObject IAPPanel, boxesPanel, rotatePanel, selectItemTypePanel, itemsPanel, settingsPanel, scrollView, shopParent, playerParent, wealthPanel, offlinePanel, statusBar, main, loadingPanel, shelfPanel;
    [SerializeField] private GridLayoutGroup actionPanel;
    [SerializeField] private Canvas canvas;

    public GameObject actionPanelBar, navButtons, pagination, openButton, itemPrice, coinImg, glow, shadow, settingsBtn, backBtn;

    public TMP_Text coins, gems, itemName, itemDesc, itemType, itemBrand, boxName, boxDesc;

    public List<TMP_Text> boxRarities;


    public string selectedShoe, selectedBall, itemsCatalogue;
    public List<string> ownedItems;

    public items items;
    public Material pallete, locked;
    public Color32[] rarityColours;
    public DatabaseManager databaseManager;
    public GameObject player, box;
    public Camera shelfCam, mainCam;
    public LoadItem loadItems;
    public LoadBoxes loadBoxes;
    public Sprite gemSprite, coinSprite;
    public RewardedAds rewardedAd;

    public int selectedBrandIndex, selectedBallBrandIndex, boxesIndex;

    public Color selectedColor = new Color(1, 1, 1, 1);

    public int shoeSide;

    public UISoundFX uISoundFX;

    public GameObject mask;

    public AndroidNotification notification;
    public int notificationID;

    void Start()
    {
        mask.transform.localScale = new Vector2(canvas.pixelRect.width, canvas.pixelRect.height * 0.54f);
        mask.transform.localPosition = new Vector3(0, -(canvas.pixelRect.height/2 - mask.transform.localScale.y/2), -500);

        selectedShoe = databaseManager.playerData.playerAttributesData.selectedShoe;
        selectedBall = databaseManager.playerData.playerAttributesData.selectedBall;
        coins.text = databaseManager.playerData.playerAttributesData.coins.ToString();
        gems.text = databaseManager.playerData.playerAttributesData.gems.ToString();

        ownedItems = new List<string>();

        foreach (var child in databaseManager.playerData.playerAttributesData.shoes)
        {
            ownedItems.Add(child.Key);
        }

        foreach (var child in databaseManager.playerData.playerAttributesData.balls)
        {
            ownedItems.Add(child.Key);
        }

        itemsCatalogue = LoadResourceTextfile("items.json");
        items = JsonConvert.DeserializeObject<items>(itemsCatalogue);

        boxesIndex = 0;
        selectedBrandIndex = 0;
        selectedBallBrandIndex = 0;

        //actionPanel.cellSize = new Vector2(350f/1440f * canvas.rect.width, 125f / 350f * 350f / 1440f * canvas.rect.width);
        //itemPrice.transform.GetChild(0).GetComponent<TMP_Text>().fontSize = actionPanel.cellSize.x * 50 / 350;
        //openButton.transform.GetChild(0).GetComponent<TMP_Text>().fontSize = actionPanel.cellSize.x * 50 / 350;

        openSelectionPanel();

        int barHeight = databaseManager.GetStatusBarHeight();
        statusBar.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1 - barHeight / canvas.pixelRect.height);
        statusBar.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        statusBar.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        statusBar.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        main.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        main.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1 - barHeight / canvas.pixelRect.height);
        main.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        main.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);


        AndroidNotificationCenter.CancelAllNotifications();

        AndroidNotification notification = new AndroidNotification();
        notification.Title = "Open Daily Box";
        notification.Text = "Time to open your daily box!";
        notification.SmallIcon = "logo_small";
        notification.LargeIcon = "shop";
        notification.Color = new Color32(0,173,173,173);
        notification.ShowTimestamp = true;
        notification.ShouldAutoCancel = true;

        DateTime date = Convert.ToDateTime(databaseManager.playerData.playerAttributesData.boxes["Dai-119304"].Split(":")[2].Replace(";", ":"));
        notification.FireTime = ((date - DateTime.UtcNow).TotalMilliseconds < 0) ? DateTime.UtcNow.AddHours(3) : date;

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

    public static string LoadResourceTextfile(string path)
    {
        string filePath = path.Replace(".json", "");

        TextAsset targetFile = Resources.Load<TextAsset>(filePath);

        return targetFile.text;
    }

    void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {

            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                previousPage();
            }
        }
    }

    public enum pageState
    {
        boxes,
        shoes,
        balls,
        none
    }

    public pageState state = pageState.none;

    public void previousPage()
    {
        if (IAPPanel.activeSelf)
        {
            IAPPanel.SetActive(false);
            settingsBtn.SetActive(true);
            backBtn.SetActive(true);
        }
        else
            SceneManager.LoadScene("Game");
    }

    public void closePanels()
    {
        box.SetActive(false);
        glow.SetActive(false);
        shadow.SetActive(false);
        player.SetActive(false);
        shelfPanel.SetActive(true);
        itemPrice.SetActive(false);
        shopParent.SetActive(true);
        openButton.SetActive(false);
        boxesPanel.SetActive(false);
        itemsPanel.SetActive(false);
        scrollView.SetActive(false);
        rotatePanel.SetActive(false);
        wealthPanel.SetActive(true);
        playerParent.SetActive(true);
        settingsPanel.SetActive(false);
        gems.gameObject.SetActive(true);
        coins.gameObject.SetActive(true);
        selectItemTypePanel.SetActive(false);
        shelfCam.gameObject.SetActive(false);

        openButton.GetComponent<Button>().onClick.RemoveAllListeners();
        itemPrice.GetComponent<Button>().onClick.RemoveAllListeners();

        LeanTween.rotate(player, new Vector3(0, 45, 0), 0.15f);
    }

    public void back()
    {
        settingsPanel.SetActive(false);
        this.GetComponent<Settings>().closePanels.ForEach(panel => panel.SetActive(true));
    }

    public void openLoading()
    {
        loadingPanel.SetActive(true);
        loadingPanel.transform.GetChild(0).localScale = Vector3.zero;
        LeanTween.scale(loadingPanel.transform.GetChild(0).gameObject, Vector3.one, 0.15f).setEaseOutBack();
    }

    public async void closeLoading()
    {
        LeanTween.scale(loadingPanel.transform.GetChild(0).gameObject, Vector3.zero, 0.15f).setEaseOutBack();
        await Task.Delay(150);
        loadingPanel.SetActive(false);
    }

    public void openOffline()
    {
        offlinePanel.SetActive(true);
        offlinePanel.transform.GetChild(0).localScale = Vector3.zero;
        LeanTween.scale(offlinePanel.transform.GetChild(0).gameObject, Vector3.one, 0.15f).setEaseOutBack();
        LeanTween.value(gameObject, new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 153), 0.15f).setOnUpdate(val => offlinePanel.GetComponent<Image>().color = val).setEaseInExpo();
    }

    public async void closeOffline()
    {
        LeanTween.scale(offlinePanel.transform.GetChild(0).gameObject, Vector3.zero, 0.15f).setEaseOutBack();
        LeanTween.value(gameObject, new Color32(0, 0, 0, 153), new Color32(0, 0, 0, 0), 0.15f).setOnUpdate(val => offlinePanel.GetComponent<Image>().color = val).setEaseInExpo();
        await Task.Delay(150);
        offlinePanel.SetActive(false);
    }

    public void tryReconnect()
    {
        SceneManager.LoadScene("Login");
    }

    public void openBoxes()
    {
        if (databaseManager.playerPrefs.offline)
        {
            openOffline();
        }
        else
        {
            closePanels();
            state = pageState.boxes;
            box.SetActive(true);
            shadow.SetActive(true);
            boxesPanel.SetActive(true);
            actionPanelBar.SetActive(true);

            loadBoxes.loadBox(boxesIndex);
        }
    }

    public void openShoes()
    {
        closePanels();
        itemType.text = "KICKS";
        glow.SetActive(true);
        player.SetActive(true);
        state = pageState.shoes;
        itemsPanel.SetActive(true);
        scrollView.SetActive(true);
        rotatePanel.SetActive(true);
        shelfCam.gameObject.SetActive(true);

        actionPanelBar.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        actionPanelBar.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        actionPanelBar.SetActive(true);
        navButtons.SetActive(true);
        pagination.SetActive(true);

        loadItems.loadShoes(selectedBrandIndex);
        List<ShoeScriptable> shoes = Resources.LoadAll<ShoeScriptable>("Scriptable Objects/Shoe/" + selectedShoe.Split(":")[0]).ToList();
        shoes.Sort((x, y) => x.id.CompareTo(y.id));
        loadItems.displayItem(shoes[int.Parse(selectedShoe.Split(":")[1])]);
    }

    public void openBalls()
    {
        closePanels();
        itemType.text = "BALLS";
        glow.SetActive(true);
        player.SetActive(true);
        state = pageState.balls;
        itemsPanel.SetActive(true);
        scrollView.SetActive(true);
        rotatePanel.SetActive(true);
        shelfCam.gameObject.SetActive(true);

        actionPanelBar.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        actionPanelBar.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        actionPanelBar.SetActive(true);
        navButtons.SetActive(true);
        pagination.SetActive(true);

        loadItems.loadBalls(selectedBallBrandIndex);
        List<BallScriptable> shoes = Resources.LoadAll<BallScriptable>("Scriptable Objects/Balls/" + selectedBall.Split(":")[0]).ToList();
        shoes.Sort((x, y) => x.id.CompareTo(y.id));
        loadItems.displayItem(shoes[int.Parse(selectedBall.Split(":")[1])]);
    }

    public void openSelectionPanel()
    {
        closePanels();
        glow.SetActive(true);
        state = pageState.none;
        player.SetActive(true);
        rotatePanel.SetActive(true);
        selectItemTypePanel.SetActive(true);
        shoeSide = databaseManager.playerPrefs.shoeSide;
        player.transform.localScale = new Vector3(1 * player.transform.localScale.x, 1 * player.transform.localScale.y, shoeSide * player.transform.localScale.z);

        actionPanelBar.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        actionPanelBar.GetComponent<RectTransform>().offsetMax = new Vector2(0, -150);
        actionPanelBar.SetActive(false);
        navButtons.SetActive(false);
        pagination.SetActive(false);

        List<ShoeScriptable> shoes = Resources.LoadAll<ShoeScriptable>("Scriptable Objects/Shoe/" + selectedShoe.Split(":")[0]).ToList();
        shoes.Sort((x, y) => x.id.CompareTo(y.id));
        loadItems.displayItem(shoes[int.Parse(selectedShoe.Split(":")[1])]);
        itemPrice.SetActive(false);
    }

    private async void wealthChange(TMP_Text txt, int start, int end, int startAmount, int amount, int multiplier)
    {
        LeanTween.value(txt.gameObject, start, end, 0.2f).setOnUpdate(val => txt.fontSize = val).setEaseOutBack();
        await Task.Delay(250);
        txt.text = (startAmount + (amount * multiplier)).ToString();
        LeanTween.value(txt.gameObject, end, start, 0.2f).setOnUpdate(val => txt.fontSize = val).setEaseInBack();
    }

    public void addCoins(int amount, int multiplier)
    {
        wealthChange(coins, 60, 80, databaseManager.playerData.playerAttributesData.coins, amount, multiplier);
        databaseManager.playerData.playerAttributesData.coins += (amount * multiplier);
        uISoundFX.playCoinChange();
        databaseManager.setCoins(databaseManager.playerData.playerAttributesData.coins);
    }

    public void addGems(int amount, int multiplier)
    {
        wealthChange(gems, 60, 80, databaseManager.playerData.playerAttributesData.gems, amount, multiplier);
        databaseManager.playerData.playerAttributesData.gems += (amount * multiplier);
        uISoundFX.playGemChange();
        databaseManager.setGems(databaseManager.playerData.playerAttributesData.gems);
    }

    public void takeCoins(int amount)
    {
        wealthChange(coins, 60, 40, databaseManager.playerData.playerAttributesData.coins, -amount, 1);
        databaseManager.playerData.playerAttributesData.coins -= amount;
        uISoundFX.playCoinChange();
        databaseManager.setCoins(databaseManager.playerData.playerAttributesData.coins);
    }

    public void takeGems(int amount)
    {
        wealthChange(gems, 60, 40, databaseManager.playerData.playerAttributesData.gems, -amount, 1);
        databaseManager.playerData.playerAttributesData.gems -= amount;
        uISoundFX.playGemChange();
        databaseManager.setGems(databaseManager.playerData.playerAttributesData.gems);
    }

    public void openIAP()
    {
        IAPPanel.SetActive(true);

        settingsBtn.SetActive(false);
        backBtn.SetActive(true);

        LeanTween.value(gameObject, new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 173), 0.25f).setOnUpdate(val => IAPPanel.GetComponent<Image>().color = val).setEaseOutCirc();
        LeanTween.value(gameObject, new Vector2(0, -canvas.pixelRect.height), Vector2.zero, 0.25f).setEaseOutBack().setOnUpdate((Vector2 val) => { IAPPanel.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = val; });
    }
}
