using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WGL_GAME : MonoBehaviour
{
    public WGL_GameInfo gameInfo;
    public WGL_Spawner spawner;

    [SerializeField] private GameObject gameOverPanel, inplayPanel;

    public WGL_SFX sfx;
    public UISoundFX uisfx;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip gameMusic, menuMusic;

    #region game
    public async void continueGame()
    {
        LeanTween.scale(gameInfo.gameOver, Vector3.zero, 0.15f);
        gameInfo.gameOver.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        await Task.Delay(150);
        gameInfo.gameOver.transform.localScale = Vector3.one;

        m_AudioSource.clip = gameMusic;
        m_AudioSource.Play();

        inplayPanel.SetActive(true);
        StopAllCoroutines();
        clearErrors();

        gameInfo.forgrounds.ForEach(go => go.SetActive(true));

        WGL_GameInfo.errors = 3;
        for (int i = 0; i < WGL_GameInfo.errors; i++)
            Instantiate(gameInfo.xObj, gameInfo.xPanel.transform);
    }

    public void inPlay()
    {
        inplayPanel.SetActive(false);
        WGL_GameInfo.inPlay = true;
        StartCoroutine(spawner.spawnItems(Random.Range(1f, 5f), Random.Range(1, 3)));
    }
    public async void gameOver()
    {
        StopAllCoroutines();
        spawner.stopSpawning();
        spawner.destroyAllItems();
        WGL_GameInfo.inPlay = false;
        gameInfo.playerPrefab.transform.parent.transform.position = Vector3.zero;
        gameInfo.gameOver.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        sfx.gameOverSound();

            uisfx.changeMenu();
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.localScale = Vector3.zero;
            LeanTween.scale(gameOverPanel, Vector3.one, 0.15f);

        gameOverPanel.SetActive(false);
        gameInfo.gameOver.SetActive(true);
        gameInfo.gameOver.transform.localScale = Vector3.zero;
        LeanTween.scale(gameInfo.gameOver, Vector3.one, 0.15f).setEaseOutBack();
        LeanTween.value(gameInfo.gameOver, new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 153), 0.15f).setOnUpdate(val => gameInfo.gameOver.GetComponent<Image>().color = val).setEaseInExpo();

        for (int i = 0; i <= WGL_GameInfo.points; i++)
        {
            gameInfo.pointsTxt.text = i.ToString();
            await Task.Delay(50);
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
        if (WGL_GameInfo.errors < 3)
        {
            gameInfo.xPanel.transform.GetChild(WGL_GameInfo.errors).GetComponent<Image>().color = new Color32(89, 89, 89, 90);
            WGL_GameInfo.errors++;
        }
    }

    public void addCoins(int amount)
    {
        wealthChange(gameInfo.coinsTxt, 60, 80, WGL_GameInfo.coins, amount);
        WGL_GameInfo.coins += amount;
        WGL_GameInfo.gameCoins += amount;
    }

    public void addGems(int amount)
    {
        wealthChange(gameInfo.gemsTxt, 60, 80, WGL_GameInfo.gems, amount);
        WGL_GameInfo.gems += amount;
        WGL_GameInfo.gameGems += amount;
    }

    public void addScore(int amount)
    {
        wealthChange(gameInfo.scoreTxt, 500, 600, WGL_GameInfo.points, amount);
        WGL_GameInfo.points += amount;
        if (WGL_GameInfo.points > WGL_GameInfo.highscore)
        {
            WGL_GameInfo.highscore = WGL_GameInfo.points;
        }
        gameInfo.highscoreTxt.text = WGL_GameInfo.highscore.ToString();
    }

    public void error()
    {
        sfx.errorSound();
        if (WGL_GameInfo.errors > 0)
        {
            //gameManager.uISounds.errorSound();
            gameInfo.xPanel.transform.GetChild(WGL_GameInfo.errors - 1).GetComponent<Image>().color = new Color32(255, 0, 139, 255);
            WGL_GameInfo.errors--;
        }

        if (WGL_GameInfo.errors == 0)
            gameOver();
    }

    public void clearErrors()
    {
        WGL_GameInfo.errors = 0;
        foreach (Transform child in gameInfo.xPanel.transform)
            Destroy(child.gameObject);
    }
    #endregion
}