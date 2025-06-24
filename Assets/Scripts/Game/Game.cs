using Google.Play.Review;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameInformation gameInfo;
    public Spawner spawner;

    [SerializeField] private TMP_Text countdown, test;
    [SerializeField] private GameObject continuePanel, gameOverPanel, prizePanel, socialsPanel, inplayPanel, IAPPanel, helpPanel, advertBtn, doublePrizePanel, loading;

    public SoundFX sfx;
    public UISoundFX uisfx;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip gameMusic, menuMusic;

    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;

    void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                back();
            }
        }
    }

    private enum GameState
    {
        mainMenu,
        gameOver
    }

    private GameState gameState = GameState.mainMenu;

    #region game
    public void startGame()
    {
        GameInformation.round = 0;
        GameInformation.points = 0;
        GameInformation.gameCoins = 0;
        GameInformation.gameGems = 0;

        gameInfo.scoreTxt.text = GameInformation.points.ToString();
        WallScript.generateWalls(1);

        continueGame();
    }

    public async void continueGame()
    {
        doublePrizePanel.SetActive(false);

        LeanTween.scale(gameInfo.mainMenu, Vector3.zero, 0.15f);
        LeanTween.scale(gameInfo.gameOver, Vector3.zero, 0.15f);
        gameInfo.gameOver.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        await Task.Delay(150);
        gameInfo.mainMenu.SetActive(false);
        gameInfo.gameOver.SetActive(false);
        gameInfo.mainMenu.transform.localScale = Vector3.one;
        gameInfo.gameOver.transform.localScale = Vector3.one;
        continuePanel.transform.localScale = Vector3.one;
        gameInformation.settingsBtn.SetActive(false);
        gameInformation.backBtn.SetActive(false);

        m_AudioSource.clip = gameMusic;
        m_AudioSource.Play();

        inplayPanel.SetActive(true);
        GameInformation.coins = gameInfo.databaseManager.playerData.playerAttributesData.coins;
        GameInformation.gems = gameInfo.databaseManager.playerData.playerAttributesData.gems;
        StopAllCoroutines();
        clearErrors();

        gameInfo.forgrounds.ForEach(go => go.SetActive(true));

        GameInformation.errors = 3;
        for (int i = 0; i < GameInformation.errors; i++)
            Instantiate(gameInfo.xObj, gameInfo.xPanel.transform);

        if (!gameInfo.databaseManager.playerData.playerAttributesData.removeAds & !gameInfo.databaseManager.playerPrefs.offline)
        {
            gameInfo.rewardedAd.DestroyAd();
            gameInfo.interstitialAd.DestroyAd();
            gameInfo.bannerAd.DestroyAd();
        }
    }

    public void inPlay()
    {
        inplayPanel.SetActive(false);
        GameInformation.inPlay = true;
        StartCoroutine(spawner.spawnItems(Random.Range(1f, 5f), Random.Range(1, 3)));
    }

    public async void continueScreen()
    {
        continuePanel.SetActive(true);
        gameOverPanel.SetActive(false);
        continuePanel.transform.localScale = Vector3.one;

        m_AudioSource.clip = menuMusic;
        m_AudioSource.Play();

        for (int i = 5; i > 0; i--)
        {
            if (!GameInformation.inPlay)
            {
                countdown.text = i.ToString();
                LeanTween.value(countdown.gameObject, 0, Mathf.PI, 0.5f).setOnUpdate(val => countdown.fontSize = (300 - Mathf.Sin(val) * 75)).setEaseInOutBack();
                sfx.playCountdownSecond();
                await Task.Delay(1000);
            }
        }

        if (!GameInformation.inPlay)
        {
            if (GameInformation.games % 2 == 0)
                showIntersitialAD();

            LeanTween.scale(continuePanel, Vector3.zero, 0.15f);
            await Task.Delay(150);
            uisfx.changeMenu();
            continuePanel.SetActive(false);
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.localScale = Vector3.zero;
            continuePanel.transform.localScale = Vector3.one;
            gameInformation.settingsBtn.SetActive(false);
            gameInformation.backBtn.SetActive(true);
            LeanTween.scale(gameOverPanel, Vector3.one, 0.15f);
            gameInfo.startTimers();
        }
    }

    public async void gameOver()
    {
        StopAllCoroutines();
        if (!gameInfo.databaseManager.playerData.playerAttributesData.removeAds & !gameInfo.databaseManager.playerPrefs.offline)
            gameInfo.bannerAd.LoadAd();

        gameState = GameState.gameOver;
        saveToDatabase();
        spawner.stopSpawning();
        spawner.destroyAllItems();
        GameInformation.inPlay = false;
        gameInfo.playerPrefab.transform.parent.transform.position = Vector3.zero;
        gameInfo.gameOver.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        sfx.gameOverSound();

        countdown.text = "5";
        continuePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameInfo.gameOver.SetActive(true);
        gameInfo.gameOver.transform.localScale = Vector3.zero;
        LeanTween.scale(gameInfo.gameOver, Vector3.one, 0.15f).setEaseOutBack();
        LeanTween.value(gameInfo.gameOver, new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 153), 0.15f).setOnUpdate(val => gameInfo.gameOver.GetComponent<Image>().color = val).setEaseInExpo();

        if ((GameInformation.gameGems > 0 | GameInformation.gameCoins > 0) & !gameInfo.databaseManager.playerPrefs.offline)
            advertBtn.SetActive(true);
        else advertBtn.SetActive(false);

        if (!gameInfo.databaseManager.playerPrefs.offline)
            continueScreen();
        else
        {
            gameInformation.settingsBtn.SetActive(false);
            gameInformation.backBtn.SetActive(true);
            continuePanel.SetActive(false);
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.localScale = Vector3.one;
        }

        GameInformation.games++;
        clearErrors();

        for (int i = 0; i <= GameInformation.points; i++)
        {
            gameInfo.pointsTxt.text = i.ToString();
            await Task.Delay(50);
        }
    }

    public async void exitGame()
    {
        switch (gameState)
        {
            case GameState.mainMenu:
                LeanTween.value(gameObject, new Color32(0, 0, 0, 173), new Color32(0, 0, 0, 0), 0.25f).setOnUpdate(val => IAPPanel.GetComponent<Image>().color = val).setEaseOutCirc();
                LeanTween.value(gameObject, Vector2.zero, new Vector2(0, -gameInformation.canvas.pixelRect.height), 0.25f).setEaseOutBack().setOnUpdate((Vector2 val) => { IAPPanel.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = val; });

                LeanTween.value(gameObject, new Color32(0, 0, 0, 173), new Color32(0, 0, 0, 0), 0.25f).setOnUpdate(val => helpPanel.GetComponent<Image>().color = val).setEaseOutCirc();
                LeanTween.value(gameObject, Vector2.zero, new Vector2(0, -gameInformation.canvas.pixelRect.height), 0.25f).setEaseOutBack().setOnUpdate((Vector2 val) => { helpPanel.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = val; });

                await Task.Delay(250);

                IAPPanel.SetActive(false);
                helpPanel.SetActive(false);
                gameInformation.settingsBtn.SetActive(true);
                gameInformation.backBtn.SetActive(false);
                break;

            case GameState.gameOver:

                if (IAPPanel.activeSelf)
                {
                    LeanTween.value(gameObject, new Color32(0, 0, 0, 173), new Color32(0, 0, 0, 0), 0.25f).setOnUpdate(val => IAPPanel.GetComponent<Image>().color = val).setEaseOutCirc();
                    LeanTween.value(gameObject, Vector2.zero, new Vector2(0, -gameInformation.canvas.pixelRect.height), 0.25f).setEaseOutBack().setOnUpdate((Vector2 val) => { IAPPanel.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = val; });

                    await Task.Delay(250);

                    IAPPanel.SetActive(false);
                    gameInformation.settingsBtn.SetActive(false);
                    gameInformation.backBtn.SetActive(true);
                }
                else
                {
                    gameState = GameState.mainMenu;
                    gameInfo.mainMenu.SetActive(true);
                    LeanTween.value(gameObject, Vector2.zero, new Vector2(gameInfo.canvas.pixelRect.width, 0), 0.15f).setOnUpdate((Vector2 val) => gameInfo.gameOver.GetComponent<RectTransform>().anchoredPosition = val).setEaseInBack();
                    LeanTween.value(gameObject, new Vector2(-gameInfo.canvas.pixelRect.width, 0), Vector2.zero, 0.15f).setOnUpdate((Vector2 val) => gameInfo.mainMenu.GetComponent<RectTransform>().anchoredPosition = val).setEaseOutBack();
                    gameInfo.gameOver.SetActive(false);
                    gameInfo.gameOver.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    gameInformation.settingsBtn.SetActive(true);
                    gameInformation.backBtn.SetActive(false);

                    WallScript.generateWalls(0);
                    GameInformation.points = 0;
                    gameInfo.scoreTxt.text = GameInformation.points.ToString();

                    gameInfo.forgrounds.ForEach(go => go.SetActive(false));
                }
                break;
        }
    }

    #endregion

    #region actions

    private async void wealthChange(TMP_Text txt, int start, int end, int startAmount, int amount)
    {
        LeanTween.value(txt.gameObject, start, end, 0.2f).setOnUpdate(val => txt.fontSize = val).setEaseOutBack();
        await Task.Delay(250);
        txt.text = (startAmount + amount).ToString();
        LeanTween.value(txt.gameObject, end, start, 0.2f).setOnUpdate(val => txt.fontSize = val).setEaseInBack();
    }

    public void addLife()
    {
        if (GameInformation.errors < 3)
        {
            gameInfo.xPanel.transform.GetChild(GameInformation.errors).GetComponent<Image>().color = new Color32(89, 89, 89, 90);
            GameInformation.errors++;
        }
    }

    public void addCoins(int amount)
    {
        wealthChange(gameInfo.coinsTxt, 60, 80, GameInformation.coins, amount);
        GameInformation.coins += amount;
        GameInformation.gameCoins += amount;
    }

    public void addGems(int amount)
    {
        wealthChange(gameInfo.gemsTxt, 60, 80, GameInformation.gems, amount);
        GameInformation.gems += amount;
        GameInformation.gameGems += amount;
    }

    public void addScore(int amount)
    {
        wealthChange(gameInfo.scoreTxt, 500, 600, GameInformation.points, amount);
        GameInformation.points += amount;
        if (GameInformation.points > GameInformation.highscore)
        {
            GameInformation.highscore = GameInformation.points;
        }
        gameInfo.highscoreTxt.text = GameInformation.highscore.ToString();
    }

    public void error()
    {
        sfx.errorSound();
        if (GameInformation.errors > 0)
        {
            //gameManager.uISounds.errorSound();
            gameInfo.xPanel.transform.GetChild(GameInformation.errors - 1).GetComponent<Image>().color = new Color32(255, 0, 139, 255);
            GameInformation.errors--;
        }

        if (GameInformation.errors == 0)
            gameOver();
    }

    public void clearErrors()
    {
        GameInformation.errors = 0;
        foreach (Transform child in gameInfo.xPanel.transform)
            Destroy(child.gameObject);
    }

    [SerializeField] private GameObject prize;
    [SerializeField] private Sprite coinSprite, gemSprite;
    public async void openPrize(int coins, int gems)
    {
        uisfx.playOpenBox();
        prizePanel.SetActive(true);
        gameInformation.settingsBtn.SetActive(false);
        gameInformation.backBtn.SetActive(false);
        LeanTween.value(gameObject, new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 173), 0.25f).setOnUpdate(val => prizePanel.GetComponent<Image>().color = val).setEaseOutCirc();
        if (coins > 0)
        {
            var coinObject = Instantiate(prize, prizePanel.transform);
            coinObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "+" + coins;
            coinObject.transform.GetChild(1).GetComponent<Image>().sprite = coinSprite;
            Vector2 randonPos = new Vector2(Random.Range(-(gameInfo.canvas.GetComponent<RectTransform>().rect.height / 2) + 500, (gameInfo.canvas.GetComponent<RectTransform>().rect.height / 2) - 500), Random.Range(-(gameInfo.canvas.GetComponent<RectTransform>().rect.height / 2) + 150, (gameInfo.canvas.GetComponent<RectTransform>().rect.height / 2) - 350));
            Color objColour = new Color(1, 1, 1, 1);
            LeanTween.value(coinObject, 1f, 0f, 1f).setOnUpdate((float val) =>
            {
                coinObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(randonPos.x * val, randonPos.y * val + ((gameInfo.canvas.GetComponent<RectTransform>().rect.height / 2) - 150) * (1f - val));
                coinObject.transform.localScale = new Vector3(1f - (0.5f * (1f - val)), 1f - (0.5f * (1f - val)), 1f - (0.5f * (1f - val)));
                coinObject.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(objColour.r, objColour.r, objColour.r, val);
                coinObject.transform.GetChild(1).GetComponent<Image>().color = new Color(objColour.r, objColour.r, objColour.r, val);
            }).setEaseInCirc().setOnComplete(() => uisfx.playCoinChange());
            addCoins(coins);
        }

        await Task.Delay(250);

        if (gems > 0)
        {
            var gemObject = Instantiate(prize, prizePanel.transform);
            gemObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "+" + gems;
            gemObject.transform.GetChild(1).GetComponent<Image>().sprite = gemSprite;
            Vector2 randonPos = new Vector2(Random.Range(-(gameInfo.canvas.GetComponent<RectTransform>().rect.height / 2) + 500, (gameInfo.canvas.GetComponent<RectTransform>().rect.height / 2) - 500), Random.Range(-(gameInfo.canvas.GetComponent<RectTransform>().rect.height / 2) + 150, (gameInfo.canvas.GetComponent<RectTransform>().rect.height / 2) - 350));
            Color objColour = new Color(1, 1, 1, 1);
            LeanTween.value(gemObject, 1f, 0f, 1f).setOnUpdate((float val) =>
            {
                gemObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(randonPos.x * val, randonPos.y * val + ((gameInfo.canvas.GetComponent<RectTransform>().rect.height / 2) - 150) * (1f - val));
                gemObject.transform.localScale = new Vector3(1f - (0.5f * (1f - val)), 1f - (0.5f * (1f - val)), 1f - (0.5f * (1f - val)));
                gemObject.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(objColour.r, objColour.r, objColour.r, val);
                gemObject.transform.GetChild(1).GetComponent<Image>().color = new Color(objColour.r, objColour.r, objColour.r, val);
            }).setEaseInCirc().setOnComplete(() => uisfx.playGemChange());
            addGems(gems);
        }

        saveToDatabase();
        await Task.Delay(1250);
        LeanTween.value(gameObject, new Color32(0, 0, 0, 173), new Color32(0, 0, 0, 0), 0.15f).setOnUpdate(val => prizePanel.GetComponent<Image>().color = val).setEaseInCirc();
        await Task.Delay(150);
        prizePanel.SetActive(false);

        switch (gameState)
        {
            case GameState.mainMenu:
                gameInformation.settingsBtn.SetActive(true);
                gameInformation.backBtn.SetActive(false);
                break;

            case GameState.gameOver:
                gameInformation.settingsBtn.SetActive(false);
                gameInformation.backBtn.SetActive(true);
                break;
        }
    }

    public void randomPrize()
    {
        if (GameInformation.giftReady)
        {
            openPrize(Random.Range(10, 50), Random.Range(5, 10));
            gameInfo.databaseManager.setGift(System.DateTime.UtcNow.ToString("MM-dd-yyyy HH;mm;ss"));
            AndroidNotificationCenter.CancelNotification(gameInformation.notificationID);
            gameInformation.notification.FireTime = System.DateTime.UtcNow.AddMinutes(30);

            gameInformation.notificationID = AndroidNotificationCenter.SendNotification(gameInformation.notification, "channel_id");

            var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(gameInformation.notificationID);


            if (notificationStatus == NotificationStatus.Scheduled)
            {
                // Replace the scheduled notification with a new notification.
                AndroidNotificationCenter.UpdateScheduledNotification(gameInformation.notificationID, gameInformation.notification, "channel_id");
            }
            else if (notificationStatus == NotificationStatus.Delivered)
            {
                // Remove the previously shown notification from the status bar.
                AndroidNotificationCenter.CancelNotification(gameInformation.notificationID);
            }
            else if (notificationStatus == NotificationStatus.Unknown)
            {
                AndroidNotificationCenter.SendNotification(gameInformation.notification, "channel_id");
            }

            gameInfo.startTimers();
        }
    }

    public void doublePrize()
    {
        openPrize(GameInformation.gameCoins, GameInformation.gameGems);
        doublePrizePanel.SetActive(false);
        advertBtn.SetActive(false);
        loading.SetActive(false);
    }

    #endregion

    #region Buttons

    public GameInformation gameInformation;
    public void openIAP()
    {
        if (!GameInformation.inPlay)
        {
            IAPPanel.SetActive(true);

            gameInformation.settingsBtn.SetActive(false);
            gameInformation.backBtn.SetActive(true);

            LeanTween.value(gameObject, new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 173), 0.25f).setOnUpdate(val => IAPPanel.GetComponent<Image>().color = val).setEaseOutCirc();
            LeanTween.value(gameObject, new Vector2(0, -gameInformation.canvas.pixelRect.height), Vector2.zero, 0.25f).setEaseOutBack().setOnUpdate((Vector2 val) => { IAPPanel.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = val;  });
        }
    }

    public void openHelp()
    {
        if (!GameInformation.inPlay)
        {
            helpPanel.SetActive(true);


            if (!gameInfo.databaseManager.playerData.playerAttributesData.removeAds & !gameInfo.databaseManager.playerPrefs.offline)
                gameInfo.bannerAd.DestroyAd();

            gameInformation.settingsBtn.SetActive(false);
            gameInformation.backBtn.SetActive(true);

            LeanTween.value(gameObject, new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 173), 0.25f).setOnUpdate(val => helpPanel.GetComponent<Image>().color = val).setEaseOutCirc();
            LeanTween.value(gameObject, new Vector2(0, -gameInformation.canvas.pixelRect.height), Vector2.zero, 0.25f).setEaseOutBack().setOnUpdate((Vector2 val) => { helpPanel.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = val; });
        }
    }

    public void openShop()
    {
        loading.SetActive(true);
        if (!gameInfo.databaseManager.playerData.playerAttributesData.removeAds & !gameInfo.databaseManager.playerPrefs.offline)
            gameInfo.bannerAd.DestroyAd();

        SceneManager.LoadScene("Shop");
    }

    public void shareGame()
    {
        new NativeShare()
            .SetSubject("Hey! Look at my highscore...")
            .SetText("I got " + gameInfo.databaseManager.playerData.playerAttributesData.highscore + " points. Can you beat that? Here's the link... \n https://play.google.com/store/apps/details?id=com.Nameles.Justforkicks&pli=1")
            .Share();
    }

    public void rateGame()
    {
        _reviewManager = new ReviewManager();
        StartCoroutine(requestReview());
        StartCoroutine(launchReview());
    }

    private IEnumerator requestReview()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();
    }

    private IEnumerator launchReview()
    {
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
    }

    public void showSocials()
    {
        if (!GameInformation.inPlay)
        {
            socialsPanel.SetActive(true);

            gameInformation.settingsBtn.SetActive(false);
            gameInformation.backBtn.SetActive(true);

            LeanTween.value(gameObject, new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 173), 0.25f).setOnUpdate(val => socialsPanel.GetComponent<Image>().color = val).setEaseOutCirc();
            LeanTween.value(gameObject, new Vector2(0, -gameInformation.canvas.pixelRect.height), Vector2.zero, 0.25f).setEaseOutBack().setOnUpdate((Vector2 val) => { socialsPanel.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = val; });
        }
    }

    public void back()
    {
        if (GameInformation.inPlay | gameState == GameState.mainMenu)
            Application.Quit();
        else
        {
            if (!gameInfo.databaseManager.playerData.playerAttributesData.removeAds & !gameInfo.databaseManager.playerPrefs.offline & helpPanel.activeSelf)
                gameInfo.bannerAd.LoadAd();

            IAPPanel.SetActive(false);
            helpPanel.SetActive(false);
            socialsPanel.SetActive(false);

            gameInfo.settings.SetActive(false);
            gameInfo.wealth.SetActive(true);
            gameInfo.playerPrefab.transform.parent.gameObject.SetActive(true);
            gameInfo.scoreTxt.gameObject.SetActive(true);
            gameInfo.walls.SetActive(true);
            GameInformation.shoeSide = gameInfo.databaseManager.playerPrefs.shoeSide;
            gameInfo.playerPrefab.transform.localScale = new Vector3(gameInfo.playerPrefab.transform.localScale.x, gameInfo.playerPrefab.transform.localScale.y, GameInformation.shoeSide * Mathf.Abs(gameInfo.playerPrefab.transform.localScale.z));

            switch (gameState)
            {
                case GameState.mainMenu:
                    gameInfo.mainMenu.SetActive(true);
                    gameInformation.settingsBtn.SetActive(true);
                    gameInformation.backBtn.SetActive(false);
                    break;

                case GameState.gameOver:
                    gameInfo.gameOver.SetActive(true);
                    gameInformation.settingsBtn.SetActive(false);
                    gameInformation.backBtn.SetActive(true);
                    break;
            }
        }
    }

    #endregion

    public void showIntersitialAD()
    {
        if (!gameInfo.databaseManager.playerData.playerAttributesData.removeAds & !gameInfo.databaseManager.playerPrefs.offline)
        {
            StartCoroutine(loadingScreen());
            gameInfo.interstitialAd.LoadAd(() => loading.SetActive(false));
        }
    }

    public void showContinueAd()
    {
        StartCoroutine(loadingScreen());
        gameInfo.rewardedAd.LoadAd(() => { }, () => loading.SetActive(false), () => { inplayPanel.SetActive(true); continuePanel.transform.localScale = Vector3.one; continueGame(); });
    }

    public void showDoubleAd()
    {
        StartCoroutine(loadingScreen());
        gameInfo.rewardedAd.LoadAd(() => { }, () => loading.SetActive(false), () => { });
        doublePrizePanel.SetActive(true);
    }

    IEnumerator loadingScreen()
    {
        loading.SetActive(true);
        yield return new WaitForSeconds(5);
        loading.SetActive(false);
    }

    public void saveToDatabase()
    {
        gameInfo.databaseManager.setHignscore(GameInformation.highscore);
        gameInfo.databaseManager.setCoins(GameInformation.coins);
        gameInfo.databaseManager.setGems(GameInformation.gems);
    }
}