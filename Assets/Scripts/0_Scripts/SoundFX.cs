using System.Collections;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    public AudioSource coinSource, gemSource, anvilSource, goSource, errorSource, goalSource, secondSource, bounceSource;
    public AudioClip coin, gem, anvil, gameOver, error, goal, second;

    GameObject eventSystem;
    private void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
    }

    public void playBounce()
    {
        bounceSource.clip = eventSystem.GetComponent<GameInformation>().bounceClips[Random.Range(0, eventSystem.GetComponent<GameInformation>().bounceClips.Length)];
        bounceSource.Play();
    }

    public void playCoin()
    {
        coinSource.clip = coin;
        coinSource.Play();
    }

    public void playGem()
    {
        gemSource.clip = gem;
        gemSource.Play();
    }

    public void playAnvil()
    {
        anvilSource.clip = anvil;
        anvilSource.Play();
    }

    public void gameOverSound()
    {
        goSource.clip = gameOver;
        goSource.Play();
    }

    public void errorSound()
    {
        errorSource.clip = error;
        errorSource.Play();
    }

    public void playGoal()
    {
        goalSource.clip = goal;
        goalSource.Play();
    }

    public void playCountdownSecond()
    {
        secondSource.clip = second;
        secondSource.Play();
    }
}
